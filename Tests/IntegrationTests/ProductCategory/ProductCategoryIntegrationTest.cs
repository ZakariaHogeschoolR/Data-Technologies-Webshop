using System.Threading.Tasks;
using ApplicationDbContext;
using Microsoft.Extensions.Configuration;
using DataTransferObject;
using Npgsql;
using Xunit;

public class ProductCategoryIntegrationTest : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public ProductCategoryIntegrationTest(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }
    private ProductCategoryRepository CreateCategoryRepository()
    {
        var db = new DatabaseConnectie(_fixture.Postgres.GetConnectionString());
        return new ProductCategoryRepository(db);
    }

    private async Task<int> SeedCategoryAsync(
        NpgsqlConnection connection,
        string name)
    {
        var sql = @"
            INSERT INTO category(name)
            VALUES(@name)
            RETURNING id;";

        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@name", name);

        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    private async Task SeedProductCategoryAsync(
        NpgsqlConnection connection,
        int productId,
        int categoryId)
    {
        var sql = @"
            INSERT INTO product_categories(product_id, category_id)
            VALUES(@productId, @categoryId);";

        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@productId", productId);
        cmd.Parameters.AddWithValue("@categoryId", categoryId);

        await cmd.ExecuteNonQueryAsync();
    }
    private ProductRepository CreateRepository()
    {
        var db = new DatabaseConnectie(_fixture.Postgres.GetConnectionString());
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Neo4j:Uri"]      = _fixture.Neo4j.GetConnectionString(),
                ["Neo4j:User"]     = "neo4j",
                ["Neo4j:Password"] = "password"
            })
            .Build();
        var neo4j = new Neo4jService(config);
        var productrep = new ProductRepository(db, neo4j);
        return productrep;
    }

    private async Task<NpgsqlConnection> OpenConnectionAsync()
    {
        var connection = new NpgsqlConnection(_fixture.Postgres.GetConnectionString());
        await connection.OpenAsync();
        return connection;
    }

    private async Task CleanProductsAsync(NpgsqlConnection connection)
    {
        await using var cmd1 = new NpgsqlCommand("DELETE FROM product_categories;", connection);
        await cmd1.ExecuteNonQueryAsync();

        await using var cmd2 = new NpgsqlCommand("DELETE FROM products;", connection);
        await cmd2.ExecuteNonQueryAsync();
    }

    private async Task CleanCategoryAsync(NpgsqlConnection connection, string name)
    {
        await using var cmd1 = new NpgsqlCommand("DELETE FROM product_categories WHERE category_id IN (SELECT id FROM category WHERE name = @name);", connection);
        cmd1.Parameters.AddWithValue("@name", name);
        await cmd1.ExecuteNonQueryAsync();

        await using var cmd2 = new NpgsqlCommand("DELETE FROM category WHERE name = @name;", connection);
        cmd2.Parameters.AddWithValue("@name", name);
        await cmd2.ExecuteNonQueryAsync();
    }

    private async Task<int> EnsureTeamAsync(NpgsqlConnection connection, string name = "Test Team", string type = "sales")
    {
        var sql = @"INSERT INTO teams (name, type)
                    VALUES (@name, @type)
                    ON CONFLICT (name) DO UPDATE SET type = EXCLUDED.type
                    RETURNING id;";
        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@type", type);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    private async Task<int> SeedProductAsync(
        NpgsqlConnection connection,
        string image, string name, string description, decimal price, int teamId)
    {
        var sql = @"INSERT INTO products (product_image, name, description, price, team_id)
                    VALUES (@productImage, @name, @description, @price, @teamId)
                    RETURNING id;";
        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@productImage", image);
        cmd.Parameters.AddWithValue("@name",         name);
        cmd.Parameters.AddWithValue("@description",  description);
        cmd.Parameters.AddWithValue("@price",        price);
        cmd.Parameters.AddWithValue("@teamId",       teamId);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    [Fact]
    public async Task GetAllProductCategorys_ShouldReturnProductsForCategory()
    {
        await using var conn = await OpenConnectionAsync();

        await CleanProductsAsync(conn);
        await CleanCategoryAsync(conn, "Electronics");

        var teamId = await EnsureTeamAsync(conn);

        var categoryId = await SeedCategoryAsync(conn, "Electronics");

        var productId = await SeedProductAsync(
            conn,
            "image.jpg",
            "Laptop",
            "Gaming laptop",
            1000m,
            teamId);

        await SeedProductCategoryAsync(conn, productId, categoryId);

        var repository = CreateCategoryRepository();

        var result = await repository.GetAllProductCategorys(categoryId);

        Assert.Single(result);

        Assert.Equal(productId, result[0].Id);
        Assert.Equal("Laptop", result[0].Name);
        Assert.Equal("Gaming laptop", result[0].Description);
        Assert.Equal(1000m, result[0].Price);
    }

    [Fact]
    public async Task GetProductsNext_ShouldReturnNextProducts()
    {
        await using var conn = await OpenConnectionAsync();

        await CleanProductsAsync(conn);
        await CleanCategoryAsync(conn, "Electronics");

        var teamId = await EnsureTeamAsync(conn);

        var categoryId = await SeedCategoryAsync(conn, "Electronics");

        var productIds = new List<int>();

        for (int i = 1; i <= 35; i++)
        {
            var productId = await SeedProductAsync(
                conn,
                $"image{i}.jpg",
                $"Product {i}",
                $"Description {i}",
                i,
                teamId);

            productIds.Add(productId);

            await SeedProductCategoryAsync(conn, productId, categoryId);
        }

        var repository = CreateCategoryRepository();

        var result = await repository.GetProductsNext(
            categoryId,
            productIds[9]); // product 10

        Assert.Equal(25, result.Count);

        Assert.All(result, p => Assert.True(p.Id > productIds[9]));
    }

    [Fact]
    public async Task GetProductsPrev_ShouldReturnPreviousProducts()
    {
        await using var conn = await OpenConnectionAsync();

        await CleanProductsAsync(conn);
        await CleanCategoryAsync(conn, "Electronics");

        var teamId = await EnsureTeamAsync(conn);

        var categoryId = await SeedCategoryAsync(conn, "Electronics");

        var productIds = new List<int>();

        for (int i = 1; i <= 35; i++)
        {
            var productId = await SeedProductAsync(
                conn,
                $"image{i}.jpg",
                $"Product {i}",
                $"Description {i}",
                i,
                teamId);

            productIds.Add(productId);

            await SeedProductCategoryAsync(conn, productId, categoryId);
        }

        var repository = CreateCategoryRepository();

        var result = await repository.GetProductsPrev(
            categoryId,
            productIds[29]); // product 30

        Assert.Equal(29, result.Count);

        Assert.All(result, p => Assert.True(p.Id < productIds[29]));

        for (int i = 1; i < result.Count; i++)
        {
            Assert.True(result[i].Id > result[i - 1].Id);
        }
    }
}