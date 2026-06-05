using System.Threading.Tasks;
using ApplicationDbContext;
using Microsoft.Extensions.Configuration;
using DataTransferObject;
using Npgsql;
using Xunit;

public class ProductIntegrationTest : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public ProductIntegrationTest(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    private (DatabaseConnectie db, ProductRepository repository) CreateRepository()
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
        return (db, new ProductRepository(db, neo4j));
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
    public async Task GetAllProducts_ShouldReturnAllSeededProducts()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanProductsAsync(connection);
        var teamId = await EnsureTeamAsync(connection);

        await SeedProductAsync(connection, "img1", "Product A", "Desc A", 77.99m, teamId);
        await SeedProductAsync(connection, "img2", "Product B", "Desc B", 12.50m, teamId);

        var results = await repository.GetAllProducts();

        Assert.Equal(2, results.Count);
        Assert.Contains(results, p => p.Name == "Product A" && p.Price == 77.99m);
        Assert.Contains(results, p => p.Name == "Product B" && p.Price == 12.50m);
    }

    [Fact]
    public async Task GetAllProducts_ShouldReturnMaxThirtyProducts()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanProductsAsync(connection);
        var teamId = await EnsureTeamAsync(connection);

        for (int i = 1; i <= 35; i++)
            await SeedProductAsync(connection, $"img{i}", $"Product {i}", $"Desc {i}", i * 1.00m, teamId);

        var results = await repository.GetAllProducts();

        Assert.Equal(30, results.Count);
    }

    [Fact]
    public async Task GetAllProductsAdmin_ShouldReturnAllProductsWithoutLimit()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanProductsAsync(connection);
        var teamId = await EnsureTeamAsync(connection);

        for (int i = 1; i <= 35; i++)
            await SeedProductAsync(connection, $"img{i}", $"Product {i}", $"Desc {i}", i * 1.00m, teamId);

        var results = await repository.GetAllProductsAdmin();

        Assert.Equal(35, results.Count);
    }

    [Fact]
    public async Task GetAllProductsAdminPaged_ShouldReturnCorrectPage()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanProductsAsync(connection);
        var teamId = await EnsureTeamAsync(connection);

        for (int i = 1; i <= 15; i++)
            await SeedProductAsync(connection, $"img{i}", $"Product {i}", $"Desc {i}", i * 1.00m, teamId);

        var page1 = await repository.GetAllProductsAdminPaged(1, 5);
        var page2 = await repository.GetAllProductsAdminPaged(2, 5);
        var page3 = await repository.GetAllProductsAdminPaged(3, 5);

        Assert.Equal(5, page1.Count);
        Assert.Equal(5, page2.Count);
        Assert.Equal(5, page3.Count);

        var page1Ids = page1.Select(p => p.Id).ToHashSet();
        var page2Ids = page2.Select(p => p.Id).ToHashSet();
        Assert.Empty(page1Ids.Intersect(page2Ids));
    }

    [Fact]
    public async Task GetProductsNext_ShouldReturnProductsAfterGivenId()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanProductsAsync(connection);
        var teamId = await EnsureTeamAsync(connection);

        var id1 = await SeedProductAsync(connection, "img1", "Product A", "Desc A", 10.00m, teamId);
        var id2 = await SeedProductAsync(connection, "img2", "Product B", "Desc B", 20.00m, teamId);
        var id3 = await SeedProductAsync(connection, "img3", "Product C", "Desc C", 30.00m, teamId);

        var results = await repository.GetProductsNext(id1);

        Assert.Equal(2, results.Count);
        Assert.All(results, p => Assert.True(p.Id > id1));
    }

    [Fact]
    public async Task GetProductsPrev_ShouldReturnProductsBeforeGivenId()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanProductsAsync(connection);
        var teamId = await EnsureTeamAsync(connection);

        var id1 = await SeedProductAsync(connection, "img1", "Product A", "Desc A", 10.00m, teamId);
        var id2 = await SeedProductAsync(connection, "img2", "Product B", "Desc B", 20.00m, teamId);
        var id3 = await SeedProductAsync(connection, "img3", "Product C", "Desc C", 30.00m, teamId);

        var results = await repository.GetProductsPrev(id3);

        Assert.Equal(2, results.Count);
        Assert.All(results, p => Assert.True(p.Id < id3));
    }

    [Fact]
    public async Task GetAllProductsByTeam_ShouldReturnOnlyProductsForThatTeam()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanProductsAsync(connection);
        var teamId1 = await EnsureTeamAsync(connection, "Team One",   "sales");
        var teamId2 = await EnsureTeamAsync(connection, "Team Two",   "sales");

        await SeedProductAsync(connection, "img1", "Product A", "Desc A", 10.00m, teamId1);
        await SeedProductAsync(connection, "img2", "Product B", "Desc B", 20.00m, teamId1);
        await SeedProductAsync(connection, "img3", "Product C", "Desc C", 30.00m, teamId2);

        var results = await repository.GetAllProductsByTeam(teamId1);

        Assert.Equal(2, results.Count);
        Assert.All(results, p => Assert.Equal(teamId1, p.TeamId));
    }

    [Fact]
    public async Task SearchProductsByName_ShouldReturnMatchingProducts()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanProductsAsync(connection);
        var teamId = await EnsureTeamAsync(connection);

        await SeedProductAsync(connection, "img1", "Apple Watch",  "Desc A", 10.00m, teamId);
        await SeedProductAsync(connection, "img2", "Apple iPhone", "Desc B", 20.00m, teamId);
        await SeedProductAsync(connection, "img3", "Samsung TV",   "Desc C", 30.00m, teamId);

        var results = await repository.SearchProductsByName("apple");

        Assert.Equal(2, results.Count);
        Assert.All(results, p => Assert.Contains("Apple", p.Name));
    }

    [Fact]
    public async Task SearchProductsByName_ShouldReturnMaxFiveProducts()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanProductsAsync(connection);
        var teamId = await EnsureTeamAsync(connection);

        for (int i = 1; i <= 8; i++)
            await SeedProductAsync(connection, $"img{i}", $"Apple {i}", $"Desc {i}", i * 1.00m, teamId);

        var results = await repository.SearchProductsByName("Apple");

        Assert.Equal(5, results.Count);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnCorrectProduct()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanProductsAsync(connection);
        var teamId = await EnsureTeamAsync(connection);

        var generatedId = await SeedProductAsync(connection, "img1", "Product A", "Desc A", 77.99m, teamId);

        var product = await repository.GetProductById(generatedId);

        Assert.NotNull(product);
        Assert.Equal(generatedId, product.Id);
        Assert.Equal("img1",      product.ProductImage);
        Assert.Equal("Product A", product.Name);
        Assert.Equal("Desc A",    product.Description);
        Assert.Equal(77.99m,      product.Price);
        Assert.Equal(teamId,      product.TeamId);
    }

    [Fact]
    public async Task GetProductById_ShouldReturnNull_WhenProductDoesNotExist()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanProductsAsync(connection);

        var product = await repository.GetProductById(99999);

        Assert.Null(product);
    }

    [Fact]
    public async Task GetProductByName_ShouldReturnMatchingProducts()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanProductsAsync(connection);
        var teamId = await EnsureTeamAsync(connection);

        await SeedProductAsync(connection, "img1", "Apple Watch",  "Desc A", 10.00m, teamId);
        await SeedProductAsync(connection, "img2", "Apple iPhone", "Desc B", 20.00m, teamId);
        await SeedProductAsync(connection, "img3", "Samsung TV",   "Desc C", 30.00m, teamId);

        var results = await repository.GetProductByName("apple");

        Assert.Equal(2, results.Count);
        Assert.All(results, p => Assert.Contains("Apple", p!.Name));
    }

    [Fact]
    public async Task GetProductByPrice_ShouldReturnProductsWithExactPrice()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanProductsAsync(connection);
        var teamId = await EnsureTeamAsync(connection);

        await SeedProductAsync(connection, "img1", "Product A", "Desc A", 50.00m, teamId);
        await SeedProductAsync(connection, "img2", "Product B", "Desc B", 99.99m, teamId);
        await SeedProductAsync(connection, "img3", "Product C", "Desc C", 50.00m, teamId);

        var results = await repository.GetProductByPrice(50.00);

        Assert.Equal(2, results.Count);
        Assert.All(results, p => Assert.Equal(50.00m, p!.Price));
    }

    [Fact]
    public async Task GetProductsByPrice_ShouldReturnProductsAtOrAbovePrice()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanProductsAsync(connection);
        var teamId = await EnsureTeamAsync(connection);

        await SeedProductAsync(connection, "img1", "Product A", "Desc A", 10.00m, teamId);
        await SeedProductAsync(connection, "img2", "Product B", "Desc B", 50.00m, teamId);
        await SeedProductAsync(connection, "img3", "Product C", "Desc C", 99.99m, teamId);

        var results = await repository.GetProductsByPrice(50.00);

        Assert.Equal(2, results.Count);
        Assert.All(results, p => Assert.True(p!.Price >= 50.00m));
    }

    [Fact]
    public async Task AddProduct_ShouldPersistProductToDatabase()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanProductsAsync(connection);
        var teamId = await EnsureTeamAsync(connection);

        var dto = new ProductDto
        {
            ProductImage = "img1",
            Name         = "Product A",
            Description  = "Desc A",
            Price        = 77.99m,
            TeamId       = teamId
        };

        await repository.AddProduct(dto);

        var selectQuery = "SELECT product_image, name, description, price, team_id FROM products WHERE name = @name;";
        await using var selectCommand = new NpgsqlCommand(selectQuery, connection);
        selectCommand.Parameters.AddWithValue("@name", dto.Name);
        await using var reader = await selectCommand.ExecuteReaderAsync();

        Assert.True(await reader.ReadAsync());
        Assert.Equal(dto.ProductImage, reader.GetString(0));
        Assert.Equal(dto.Name,         reader.GetString(1));
        Assert.Equal(dto.Description,  reader.GetString(2));
        Assert.Equal(dto.Price,        reader.GetDecimal(3));
        Assert.Equal(dto.TeamId,       reader.GetInt32(4));
    }

    [Fact]
    public async Task UpdateProduct_ShouldPersistChangesToDatabase()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanProductsAsync(connection);
        var teamId = await EnsureTeamAsync(connection);

        var generatedId = await SeedProductAsync(connection, "img1", "Old Name", "Old Desc", 10.00m, teamId);

        var dto = new ProductDto
        {
            Id           = generatedId,
            ProductImage = "img_updated",
            Name         = "New Name",
            Description  = "New Desc",
            Price        = 99.99m,
            TeamId       = teamId
        };

        await repository.UpdateProduct(dto);

        var selectQuery = "SELECT product_image, name, description, price FROM products WHERE id = @id;";
        await using var selectCommand = new NpgsqlCommand(selectQuery, connection);
        selectCommand.Parameters.AddWithValue("@id", generatedId);
        await using var reader = await selectCommand.ExecuteReaderAsync();

        Assert.True(await reader.ReadAsync());
        Assert.Equal("img_updated", reader.GetString(0));
        Assert.Equal("New Name",    reader.GetString(1));
        Assert.Equal("New Desc",    reader.GetString(2));
        Assert.Equal(99.99m,        reader.GetDecimal(3));
    }

    [Fact]
    public async Task UpdatePrice_ShouldUpdateOnlyThePrice()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanProductsAsync(connection);
        var teamId = await EnsureTeamAsync(connection);

        var generatedId = await SeedProductAsync(connection, "img1", "Product A", "Desc A", 10.00m, teamId);

        await repository.UpdatePrice(generatedId, 49.99m);

        var selectQuery = "SELECT price, name FROM products WHERE id = @id;";
        await using var selectCommand = new NpgsqlCommand(selectQuery, connection);
        selectCommand.Parameters.AddWithValue("@id", generatedId);
        await using var reader = await selectCommand.ExecuteReaderAsync();

        Assert.True(await reader.ReadAsync());
        Assert.Equal(49.99m,      reader.GetDecimal(0));
        Assert.Equal("Product A", reader.GetString(1)); // other fields unchanged
    }

    [Fact]
    public async Task DeleteProduct_ShouldRemoveProductFromDatabase()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanProductsAsync(connection);
        var teamId = await EnsureTeamAsync(connection);

        var generatedId = await SeedProductAsync(connection, "img1", "Product A", "Desc A", 77.99m, teamId);

        await repository.DeleteProduct(generatedId);

        var selectQuery = "SELECT name FROM products WHERE id = @id;";
        await using var selectCommand = new NpgsqlCommand(selectQuery, connection);
        selectCommand.Parameters.AddWithValue("@id", generatedId);
        var retrievedName = (string?)await selectCommand.ExecuteScalarAsync();

        Assert.Null(retrievedName);
    }

    [Fact]
    public async Task AddProductScrape_ShouldPersistProductAndReturnGeneratedId()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanProductsAsync(connection);
        var teamId = await EnsureTeamAsync(connection);

        var dto = new ProductDto
        {
            ProductImage = "img1",
            Name         = "Scraped Product",
            Description  = "Scraped Desc",
            Price        = 19.99m,
            TeamId       = teamId
        };

        var returnedId = await repository.AddProductScrape(dto);

        Assert.True(returnedId > 0);

        var selectQuery = "SELECT name, price FROM products WHERE id = @id;";
        await using var selectCommand = new NpgsqlCommand(selectQuery, connection);
        selectCommand.Parameters.AddWithValue("@id", returnedId);
        await using var reader = await selectCommand.ExecuteReaderAsync();

        Assert.True(await reader.ReadAsync());
        Assert.Equal("Scraped Product", reader.GetString(0));
        Assert.Equal(19.99m,            reader.GetDecimal(1));
    }

    [Fact]
    public async Task AddProductScrape_ShouldReturnMinusOne_WhenImageIsEmpty()
    {
        var (_, repository) = CreateRepository();

        var dto = new ProductDto
        {
            ProductImage = "",
            Name         = "No Image Product",
            Description  = "Desc",
            Price        = 9.99m,
            TeamId       = 1
        };

        var result = await repository.AddProductScrape(dto);

        Assert.Equal(-1, result);
    }

    [Fact]
    public async Task GetOrCreateTeam_ShouldCreateTeamAndReturnId()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();

        await using var cleanupCmd = new NpgsqlCommand("DELETE FROM teams WHERE name = @name;", connection);
        cleanupCmd.Parameters.AddWithValue("@name", "Brand New Team");
        await cleanupCmd.ExecuteNonQueryAsync();

        var id = await repository.GetOrCreateTeam("Brand New Team", "sales");

        Assert.True(id > 0);

        var selectQuery = "SELECT name, type FROM teams WHERE id = @id;";
        await using var selectCommand = new NpgsqlCommand(selectQuery, connection);
        selectCommand.Parameters.AddWithValue("@id", id);
        await using var reader = await selectCommand.ExecuteReaderAsync();

        Assert.True(await reader.ReadAsync());
        Assert.Equal("Brand New Team", reader.GetString(0));
        Assert.Equal("sales",          reader.GetString(1));
    }

    [Fact]
    public async Task GetOrCreateTeam_ShouldReturnSameId_WhenTeamAlreadyExists()
    {
        var (_, repository) = CreateRepository();

        var firstId  = await repository.GetOrCreateTeam("Existing Team", "sales");
        var secondId = await repository.GetOrCreateTeam("Existing Team", "sales");

        Assert.Equal(firstId, secondId);
    }

    [Fact]
    public async Task GetOrCreateCategory_ShouldCreateCategoryAndReturnId()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanCategoryAsync(connection, "Electronics");

        var id = await repository.GetOrCreateCategory("Electronics");

        Assert.True(id > 0);

        var selectQuery = "SELECT name FROM category WHERE id = @id;";
        await using var selectCommand = new NpgsqlCommand(selectQuery, connection);
        selectCommand.Parameters.AddWithValue("@id", id);
        var retrievedName = (string?)await selectCommand.ExecuteScalarAsync();

        Assert.Equal("Electronics", retrievedName);
    }

    [Fact]
    public async Task GetOrCreateCategory_ShouldReturnSameId_WhenCategoryAlreadyExists()
    {
        var (_, repository) = CreateRepository();

        var firstId  = await repository.GetOrCreateCategory("Existing Category");
        var secondId = await repository.GetOrCreateCategory("Existing Category");

        Assert.Equal(firstId, secondId);
    }

    [Fact]
    public async Task AddProductCategory_ShouldLinkProductToCategory()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanProductsAsync(connection);
        var teamId = await EnsureTeamAsync(connection);

        var productId  = await SeedProductAsync(connection, "img1", "Product A", "Desc A", 10.00m, teamId);
        var categoryId = await repository.GetOrCreateCategory("Electronics");

        await repository.AddProductCategory(productId, categoryId);

        var selectQuery = "SELECT product_id, category_id FROM product_categories WHERE product_id = @productId AND category_id = @categoryId;";
        await using var selectCommand = new NpgsqlCommand(selectQuery, connection);
        selectCommand.Parameters.AddWithValue("@productId",  productId);
        selectCommand.Parameters.AddWithValue("@categoryId", categoryId);
        await using var reader = await selectCommand.ExecuteReaderAsync();

        Assert.True(await reader.ReadAsync());
        Assert.Equal(productId,  reader.GetInt32(0));
        Assert.Equal(categoryId, reader.GetInt32(1));
    }

    [Fact]
    public async Task AddProductCategory_ShouldNotThrow_WhenLinkAlreadyExists()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanProductsAsync(connection);
        var teamId = await EnsureTeamAsync(connection);

        var productId  = await SeedProductAsync(connection, "img1", "Product A", "Desc A", 10.00m, teamId);
        var categoryId = await repository.GetOrCreateCategory("Electronics");

        await repository.AddProductCategory(productId, categoryId);

        var exception = await Record.ExceptionAsync(() =>
            repository.AddProductCategory(productId, categoryId));

        Assert.Null(exception);
    }

    [Fact]
    public async Task GetProductsByCategories_ShouldReturnOnlyProductsInGivenCategories()
    {
        var (_, repository) = CreateRepository();
        await using var connection = await OpenConnectionAsync();
        await CleanProductsAsync(connection);
        var teamId = await EnsureTeamAsync(connection);

        var productId1 = await SeedProductAsync(connection, "img1", "Product A", "Desc A", 10.00m, teamId);
        var productId2 = await SeedProductAsync(connection, "img2", "Product B", "Desc B", 20.00m, teamId);
        var productId3 = await SeedProductAsync(connection, "img3", "Product C", "Desc C", 30.00m, teamId);

        var categoryId1 = await repository.GetOrCreateCategory("Electronics");
        var categoryId2 = await repository.GetOrCreateCategory("Clothing");

        await repository.AddProductCategory(productId1, categoryId1);
        await repository.AddProductCategory(productId2, categoryId1);
        await repository.AddProductCategory(productId3, categoryId2);

        var results = await repository.GetProductsByCategories(new List<int> { categoryId1 }, 1, 10);

        Assert.Equal(2, results.Count);
        Assert.All(results, p => Assert.NotEqual(productId3, p.Id));
    }
}