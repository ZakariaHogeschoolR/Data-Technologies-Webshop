using System.Data.Common;
using System.Reflection;
using ApplicationDbContext;   // for DatabaseConnectie
using DataTransferObject;

using Npgsql;     // for WishlistDTO

public class WishlistIntegrationTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private WishlistRepository repository;
    public WishlistIntegrationTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        var db = new DatabaseConnectie(fixture.Postgres.GetConnectionString());
        repository = new WishlistRepository(db);
    }

    [Fact]
    public async Task AddWishlist_Succes()
    {
        var connectionString = _fixture.Postgres.GetConnectionString();
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        // Insert a user to satisfy the foreign key on user_id
        await using var userCmd = new NpgsqlCommand(
            "INSERT INTO users (username, email, password) VALUES ('testuser', 'test@test.com', 'hash') ON CONFLICT DO NOTHING RETURNING id;",
            connection);
        var userId = (int)(await userCmd.ExecuteScalarAsync() ?? 1);

        // Insert a product to satisfy the foreign key on product_id
        await using var productCmd = new NpgsqlCommand(
            "INSERT INTO product (name, price) VALUES ('Test Product', 9.99) RETURNING id;",
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
}
