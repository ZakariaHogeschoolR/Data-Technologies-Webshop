using ApplicationDbContext;

using DataTransferObject;

using Microsoft.VisualBasic;

using Npgsql;

public class ShoppingCartIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private string connectionString;
    public ShoppingCartIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        connectionString = _fixture.Postgres.GetConnectionString();
    }
    [Fact]
    public async Task AddShoppingCart_ShouldSucceed()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var (userId, productId) = await SeedRequiredData(connection);

        var db = new DatabaseConnectie(connectionString);
        var repository = new ShoppingCartRepository(db);

        var dto = new ShoppingCartDTO
        {
            UserId = userId,
            ProductId = productId,
            Quantity = 2
        };

        var result = await repository.AddShoppingCarts(dto);

        Assert.NotNull(result);
        Assert.Equal(productId, result.ProductId);
        Assert.Equal(2, result.Quantity);
    }

    [Fact]
    public async Task AddShoppinCart_SameProductTwice_ShouldIncreaseQuantity()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var (userId, productId) = await SeedRequiredData(connection);

        var db = new DatabaseConnectie(connectionString);
        var repository = new ShoppingCartRepository(db);

        var dto = new ShoppingCartDTO
        {
            UserId = userId,
            ProductId = productId,
            Quantity = 2
        };
        await repository.AddShoppingCarts(dto);
        var result = await repository.AddShoppingCarts(dto);

        Assert.Equal(4, result.Quantity);
    }

    [Fact]
    public async Task GetShoppingCartById_ShouldReturnItemsForUser()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var (userId, productId) = await SeedRequiredData(connection);

        var db = new DatabaseConnectie(connectionString);
        var repository = new ShoppingCartRepository(db);

        await repository.AddShoppingCarts(new ShoppingCartDTO
        {
            UserId = userId,
            ProductId = productId,
            Quantity = 2
        });

        var result = await repository.GetShoppingCartById(userId);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, c => c?.ProductId == productId);
    }

    [Fact]
    public async Task GetShoppingCartById_ShouldReturnEmpty_WhenUserHasNoCart()
    {
        await using var connection =  new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var (userId, _) = await SeedRequiredData(connection);

        var db = new DatabaseConnectie(connectionString);
        var repository = new ShoppingCartRepository(db);

        var result = await repository.GetOrderHistoryByUserId(userId);

        Assert.True(result == null || result.Count == 0);
    }

    [Fact]
    public async Task GetAllShoppingCarts_ShouldReturnAtleastAddedCarts()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var (userId, productId) = await SeedRequiredData(connection);

        var db = new DatabaseConnectie(connectionString);
        var repository = new ShoppingCartRepository(db);

        await repository.AddShoppingCarts(new ShoppingCartDTO
        {
            UserId = userId,
            ProductId = productId,
            Quantity = 1
        });

        var result = await repository.GetAllShoppingCarts();

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains(result, c => c?.ProductId == productId);
    }

    [Fact]
    public async Task DeleteShoppingCarts_ShouldRemoveAllItemsForUser()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var (userId, productId) = await SeedRequiredData(connection);

        var db = new DatabaseConnectie(connectionString);
        var repository = new ShoppingCartRepository(db);

        await repository.AddShoppingCarts(new ShoppingCartDTO
        {
            UserId = userId,
            ProductId = productId,
            Quantity = 1
        });
        await repository.DeleteShoppingCarts(userId);

        var result = await repository.GetShoppingCartById(userId);

        Assert.Empty(result);
    }

    [Fact]
    public async Task DeleteProductFromShoppingCarts_ShouldRemoveOnlyThatProduct()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var (userId, productId) = await SeedRequiredData(connection);

        await using var TeamCmd = new NpgsqlCommand(
            "SELECT id FROM teams LIMIT 1;",
            connection
        );
        var TeamId = (int)(await TeamCmd.ExecuteScalarAsync())!;

        await using var productCmd2 = new NpgsqlCommand(
            "INSERT INTO products (name, price, team_id) VALUES (@name, @price, @team_id) RETURNING id;",
            connection
        );
        productCmd2.Parameters.AddWithValue("@name", "Second Product");
        productCmd2.Parameters.AddWithValue("@price", 4.99m);
        productCmd2.Parameters.AddWithValue("@team_id", TeamId);
        var productId2 = (int)(await productCmd2.ExecuteScalarAsync())!;

        var db = new DatabaseConnectie(connectionString);
        var repository = new ShoppingCartRepository(db);

        await repository.AddShoppingCarts(new ShoppingCartDTO
        {
            UserId = userId,
            ProductId = productId,
            Quantity = 1
        });
        await repository.AddShoppingCarts(new ShoppingCartDTO
        {
            UserId = userId,
            ProductId = productId2,
            Quantity = 1
        });

        await repository.DeleteProductFromShoppingcarts(new ShoppingCartDTO
        {
            Id = userId,
            ProductId = productId,
            Quantity = 0
        });

        var result = await repository.GetShoppingCartById(userId);

        Assert.DoesNotContain(result, c => c?.ProductId == productId);
        Assert.Contains(result, c => c?.ProductId == productId2);
    }

    [Fact]
    public async Task Checkout_ShouldCreateOrderAndClearCart()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var (userId, productId) = await SeedRequiredData(connection);

        var db = new DatabaseConnectie(connectionString);
        var repository = new ShoppingCartRepository(db);

        await repository.AddShoppingCarts(new ShoppingCartDTO
        {
            UserId = userId,
            ProductId = productId,
            Quantity = 2
        });

        var result = await repository.Checkout(userId, "card");

        Assert.NotNull(result);
        Assert.True(result.OrderId > 0);
        Assert.True(result.Total > 0);
        Assert.NotEmpty(result.Items);

        var cartAfter = await repository.GetShoppingCartById(userId);
        Assert.Empty(cartAfter);
    }

    [Fact]
    public async Task Checkout_ShouldThrow_WhenCartIsEmpty()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var (userId, _) = await SeedRequiredData(connection);

        var db = new DatabaseConnectie(connectionString);
        var repository = new ShoppingCartRepository(db);

        await Assert.ThrowsAsync<Exception>(() => repository.Checkout(userId, "card"));
    }

    private async Task<(int UserId, int ProductId)> SeedRequiredData(NpgsqlConnection connection)
    {
        await using var UserCmd = new NpgsqlCommand(
            "INSERT INTO users (username, email, password) VALUES (@username, @email, @password) RETURNING id;",
            connection);
        UserCmd.Parameters.AddWithValue("@username", Guid.NewGuid().ToString());
        UserCmd.Parameters.AddWithValue("@email", Guid.NewGuid().ToString() + "@test.com");
        UserCmd.Parameters.AddWithValue("@password", "hash");
        var UserId = (int)(await UserCmd.ExecuteScalarAsync())!;

        await using var TeamCmd = new NpgsqlCommand(
            "INSERT INTO teams (name, type) VALUES (@name,@type) RETURNING id",
            connection
        );
        TeamCmd.Parameters.AddWithValue("@name", Guid.NewGuid().ToString());
        TeamCmd.Parameters.AddWithValue("@type", "TestType");
        var TeamId = (int)(await TeamCmd.ExecuteScalarAsync())!;

        await using var ProductCmd = new NpgsqlCommand(
            "INSERT INTO products (name, price, team_id) VALUES(@name, @price,@team_id) RETURNING id;",
            connection
        );
        ProductCmd.Parameters.AddWithValue("@name", "Test Product");
        ProductCmd.Parameters.AddWithValue("@price", 9.99m);
        ProductCmd.Parameters.AddWithValue("@team_id", TeamId);
        var productId = (int)(await ProductCmd.ExecuteScalarAsync())!;

        return (UserId, productId);
    }
}
