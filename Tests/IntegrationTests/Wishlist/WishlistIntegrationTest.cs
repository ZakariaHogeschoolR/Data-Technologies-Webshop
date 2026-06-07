using ApplicationDbContext;   // for DatabaseConnectie

using DataTransferObject;

using Npgsql;     // for WishlistDTO

public class WishlistIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private string connectionString;
    public WishlistIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        connectionString = _fixture.Postgres.GetConnectionString();
    }

    [Fact]
    public async Task AddWishlist_Succes()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var userCmd = new NpgsqlCommand(
            "INSERT INTO users (username, email, password) VALUES ('testuser', 'test@test.com', 'hash') ON CONFLICT DO NOTHING RETURNING id;",
            connection);
        var userId = (int)(await userCmd.ExecuteScalarAsync() ?? 1);

        await using var teamCmd = new NpgsqlCommand(
            "INSERT INTO teams (name, type) VALUES ('TestTeam', 'TestType') RETURNING id;",
            connection);
        var teamId = (int)(await teamCmd.ExecuteScalarAsync())!;

        await using var productCmd = new NpgsqlCommand(
            "INSERT INTO products (name, price) VALUES ('Test Product', 9.99) RETURNING id;",
            connection);
        var productId = (int)(await productCmd.ExecuteScalarAsync())!;

        var db = new DatabaseConnectie(connectionString);
        var repository = new WishlistRepository(db);

        var dto = new WishlistDTO
        {
            Name = "Game",
            UserId = userId,
            ProductId = productId
        };

        var result = await repository.AddWishlist(dto);

        Assert.NotNull(result);
        Assert.Equal("Game", result.Name);
        Assert.Equal(userId, result.Userid);
        Assert.Equal(productId, result.Productid);
    }

    [Fact]
    public async Task GetAllWishlists_ShouldReturnAllWishlists()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var (userId, productId) = await SeedRequiredData(connection);
        var db = new DatabaseConnectie(connectionString);
        var repository = new WishlistRepository(db);

        await repository.AddWishlist(new WishlistDTO { Name = "List1", UserId = userId, ProductId = productId });
        await repository.AddWishlist(new WishlistDTO { Name = "List2", UserId = userId, ProductId = productId });

        var result = await repository.GetAllWishLists();

        Assert.NotNull(result);
        Assert.True(result.Count >= 2);
        Assert.Contains(result, w => w.Name == "List1");
        Assert.Contains(result, w => w.Name == "List2");
    }

    [Fact]
    public async Task GetWishlistaById_ShouldReturnCorrectWishlists()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var (userId, productId) = await SeedRequiredData(connection);

        var db = new DatabaseConnectie(connectionString);
        var repository = new WishlistRepository(db);

        var added = await repository.AddWishlist(new WishlistDTO
        {
            Name = "MyList",
            UserId = userId,
            ProductId = productId
        });

        var result = await repository.GetWishlistsById(added!.Id);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.All(result, w => Assert.Equal("MyList", w?.Name));
    }

    [Fact]
    public async Task GetWishlistsByUserId_ShouldReturnWishlistsForUser()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var (userId1, productId) = await SeedRequiredData(connection);
        var (userId2, _) = await SeedRequiredData(connection);

        var db = new DatabaseConnectie(connectionString);
        var repository = new WishlistRepository(db);

        await repository.AddWishlist(new WishlistDTO { Name = "User1List", UserId = userId1, ProductId = productId });
        await repository.AddWishlist(new WishlistDTO { Name = "User2List", UserId = userId2, ProductId = productId });

        var result = await repository.GetWishlistsByUserId(userId1);

        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.All(result, w => Assert.Equal(userId1, w?.Userid));
        Assert.DoesNotContain(result, w => w?.Userid == userId2);
    }

    [Fact]
    public async Task UpdateWishlist_ShouldPersist()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var (userId, productId) = await SeedRequiredData(connection);

        var db = new DatabaseConnectie(connectionString);
        var repository = new WishlistRepository(db);

        var added = await repository.AddWishlist(new WishlistDTO
        {
            Name = "OldName",
            UserId = userId,
            ProductId = productId
        });

        await repository.UpdateWishlist(new WishlistDTO
        {
            Id = added!.Id,
            Name = "NewName",
            UserId = userId,
            ProductId = productId
        });

        await using var cmd = new NpgsqlCommand(
            "SELECT name FROM wishlist WHERE id = @id;",
            connection);
        cmd.Parameters.AddWithValue("@id", added.Id);
        var updatedName = (string?)await cmd.ExecuteScalarAsync();

        Assert.Equal("NewName", updatedName);
    }

    [Fact]
    public async Task DeleteWishlist_ShouldRemoveFromDatabase()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var (userId, productId) = await SeedRequiredData(connection);

        var db = new DatabaseConnectie(connectionString);
        var repository = new WishlistRepository(db);

        var added = repository.AddWishlist(new WishlistDTO
        {
            Name = "ToDelete",
            UserId = userId,
            ProductId = productId
        });

        await repository.DeleteWishlist(added!.Id);

        await using var cmd = new NpgsqlCommand(
            "SELECT COUNT(*) FROM wishlist WHERE id = @id;",
            connection);
        cmd.Parameters.AddWithValue("@id", added.Id);
        var count = (long)(await cmd.ExecuteScalarAsync())!;

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task DeleteProduct_ShouldSetProductIdToNull()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var (userId, productId) = await SeedRequiredData(connection);

        var db = new DatabaseConnectie(connectionString);
        var repository = new WishlistRepository(db);

        await repository.AddWishlist(new WishlistDTO
        {
            Name = "GameList",
            UserId = userId,
            ProductId = productId
        });

        await repository.DeleteProduct(productId, "GameList");

        await using var cmd = new NpgsqlCommand(
            "SELECT product_id FROM wishlist WHERE user_id = @userId AND name = @name;",
            connection);
        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Parameters.AddWithValue("@name", "GameList");
        var result = await cmd.ExecuteScalarAsync();

        Assert.True(result == DBNull.Value || result == null);
    }

    [Fact]
    public async Task GetWishlistByUserId_ShouldReturnEmpty_WhenUserHasNoWishlists()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var (userId, _) = await SeedRequiredData(connection);

        var db = new DatabaseConnectie(connectionString);
        var repository = new WishlistRepository(db);

        var result = await repository.GetWishlistsByUserId(userId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAllWishlists_ShouldReturnEmpty_WhenTableIsEmpty()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using var deleteCmd = new NpgsqlCommand(
            "DELETE FROM wishlist;",
            connection
        );
        await deleteCmd.ExecuteNonQueryAsync();

        var db = new DatabaseConnectie(connectionString);
        var repository = new WishlistRepository(db);

        var result = await repository.GetAllWishLists();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task AddWishlist_WithNullProductId_ShouldSucced()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var (userId, _) = await SeedRequiredData(connection);

        var db = new DatabaseConnectie(connectionString);
        var repository = new WishlistRepository(db);

        var dto = new WishlistDTO
        {
            Name = "NoProductList",
            UserId = userId,
            ProductId = null
        };
        var result = await repository.AddWishlist(dto);

        Assert.NotNull(result);
        Assert.Equal("NoProductList", result.Name);
        Assert.Null(result.Productid);
    }

    [Fact]
    public async Task GetWishlistById_ShouldReturnEmpty_WhenIdDoesNotExist()
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var db = new DatabaseConnectie(connectionString);
        var repository = new WishlistRepository(db);

        var result = await repository.GetWishlistsByUserId(999999);

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    private async Task<(int userId, int productId)> SeedRequiredData(NpgsqlConnection connection)
    {
        await using var userCmd = new NpgsqlCommand(
            "INSERT INTO users (username, email, password) VALUES (@username, @email, @password) RETURNING id;",
            connection);
        userCmd.Parameters.AddWithValue("@username", Guid.NewGuid().ToString());
        userCmd.Parameters.AddWithValue("@email", Guid.NewGuid().ToString() + "@test.com");
        userCmd.Parameters.AddWithValue("@password", "hash");
        var userId = (int)(await userCmd.ExecuteScalarAsync())!;

        await using var teamCmd = new NpgsqlCommand(
            "INSERT INTO teams (name, type) VALUES (@name, @type) RETURNING id;",
            connection);
        teamCmd.Parameters.AddWithValue("@name", Guid.NewGuid().ToString());
        teamCmd.Parameters.AddWithValue("@type", "TestType");
        var teamId = (int)(await teamCmd.ExecuteScalarAsync())!;

        await using var productCmd = new NpgsqlCommand(
            "INSERT INTO products (name, price, team_id) VALUES (@name, @price, @teamId) RETURNING id;",
            connection);
        productCmd.Parameters.AddWithValue("@name", "Test Product");
        productCmd.Parameters.AddWithValue("@price", 9.99m);
        productCmd.Parameters.AddWithValue("@teamId", teamId);
        var productId = (int)(await productCmd.ExecuteScalarAsync())!;

        return (userId, productId);
    }
}
