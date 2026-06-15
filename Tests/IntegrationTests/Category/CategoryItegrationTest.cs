using System.Data.Common;
using System.Data.SqlTypes;
using System.Reflection;
using System.Threading.Tasks;

using ApplicationDbContext;

using DataTransferObject;

using Microsoft.Extensions.Configuration;

using Npgsql;

using Xunit;

public class CategoryIntegrationTest : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    // xUnit zorgt ervoor dat de DatabaseFixture hier automatisch wordt binnengestuurd
    public CategoryIntegrationTest(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAllCategory_ShouldSaveAndRetrieveCorrectData()
    {
        var connectionString = _fixture.Postgres.GetConnectionString();
        var testCategoryName = "Elektronica";

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var cleanupCommand =
            new NpgsqlCommand("DELETE FROM category;", connection);
        await cleanupCommand.ExecuteNonQueryAsync();
        var insertQuery = "INSERT INTO category (Name) VALUES (@name) RETURNING Id;";

        await using var insertCommand = new NpgsqlCommand(insertQuery, connection);
        insertCommand.Parameters.AddWithValue("@name", testCategoryName);

        var generatedId = await insertCommand.ExecuteScalarAsync();

        var selectQuery = "SELECT Name FROM category WHERE Id = @id;";
        await using var selectCommand = new NpgsqlCommand(selectQuery, connection);
        selectCommand.Parameters.AddWithValue("@id", generatedId);

        var retrievedName = (string?)await selectCommand.ExecuteScalarAsync();

        Assert.NotNull(retrievedName);
        Assert.Equal(testCategoryName, retrievedName);
    }

    [Fact]
    public async Task GetCategoryById_ShouldSaveAndRetrieveCorrectData()
    {
        var connectionString = _fixture.Postgres.GetConnectionString();
        var connectionNeo4jString = _fixture.Neo4j.GetConnectionString();
        var db = new DatabaseConnectie(connectionString);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Neo4j:Uri"] = _fixture.Neo4j.GetConnectionString(),
                ["Neo4j:User"] = "neo4j",
                ["Neo4j:Password"] = "password"
            })
            .Build();

        var neo4j = new Neo4jService(config);
        var testCategoryName = "Elektronica";

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var cleanupCommand =
            new NpgsqlCommand("DELETE FROM category;", connection);
        await cleanupCommand.ExecuteNonQueryAsync();
        var insertQuery = "INSERT INTO category (Name) VALUES (@name) RETURNING Id;";

        await using var insertCommand = new NpgsqlCommand(insertQuery, connection);
        insertCommand.Parameters.AddWithValue("@name", testCategoryName);

        var generatedId = await insertCommand.ExecuteScalarAsync();

        var selectQuery = "SELECT Name FROM category WHERE Id = @id;";
        await using var selectCommand = new NpgsqlCommand(selectQuery, connection);
        selectCommand.Parameters.AddWithValue("@id", generatedId);

        var retrievedName = (string?)await selectCommand.ExecuteScalarAsync();

        var repository = new CategoryRepository(db, neo4j);
        var category = await repository.GetCategoryById((int)generatedId);

        Assert.NotNull(retrievedName);
        Assert.Equal(testCategoryName, retrievedName);
        Assert.Equal(category.Name, retrievedName);
    }

    [Fact]
    public async Task InsertCategory_ShouldSaveAndRetrieveCorrectData()
    {
        var connectionString = _fixture.Postgres.GetConnectionString();
        var connectionNeo4jString = _fixture.Neo4j.GetConnectionString();
        var db = new DatabaseConnectie(connectionString);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Neo4j:Uri"] = _fixture.Neo4j.GetConnectionString(),
                ["Neo4j:User"] = "neo4j",
                ["Neo4j:Password"] = "password"
            })
            .Build();

        var neo4j = new Neo4jService(config);
        var testCategoryName = "Elektronica";

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var cleanupCommand =
            new NpgsqlCommand("DELETE FROM category;", connection);
        await cleanupCommand.ExecuteNonQueryAsync();
        var insertQuery = "INSERT INTO category (Name) VALUES (@name) RETURNING Id;";

        await using var insertCommand = new NpgsqlCommand(insertQuery, connection);
        insertCommand.Parameters.AddWithValue("@name", testCategoryName);

        var generatedId = await insertCommand.ExecuteScalarAsync();

        var selectQuery = "SELECT Name FROM category WHERE Id = @id;";
        await using var selectCommand = new NpgsqlCommand(selectQuery, connection);
        selectCommand.Parameters.AddWithValue("@id", generatedId);

        var retrievedName = (string?)await selectCommand.ExecuteScalarAsync();

        var repository = new CategoryRepository(db, neo4j);
        var dto = new CategoryDto
        {
            Id = (int)generatedId + 1,
            Name = "Electricity2"
        };
        await repository.AddCategory(dto);
        var selectRepQuery = "SELECT Name FROM category WHERE name = @name;";
        await using var selectRepCommand = new NpgsqlCommand(selectRepQuery, connection);
        selectRepCommand.Parameters.AddWithValue("@name", dto.Name);
        var retrievedRepName = (string?)await selectRepCommand.ExecuteScalarAsync();

        Assert.NotNull(retrievedName);
        Assert.Equal(testCategoryName, retrievedName);
        Assert.Equal(dto.Name, retrievedRepName);
    }

    [Fact]
    public async Task UpdateCategory_ShouldSaveAndRetrieveCorrectData()
    {
        var connectionString = _fixture.Postgres.GetConnectionString();
        var connectionNeo4jString = _fixture.Neo4j.GetConnectionString();
        var db = new DatabaseConnectie(connectionString);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Neo4j:Uri"] = _fixture.Neo4j.GetConnectionString(),
                ["Neo4j:User"] = "neo4j",
                ["Neo4j:Password"] = "password"
            })
            .Build();

        var neo4j = new Neo4jService(config);
        var testCategoryName = "Electronica";
        var testCategoryUpdateName = "Tshirt";

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var cleanupCommand =
            new NpgsqlCommand("DELETE FROM category;", connection);
        await cleanupCommand.ExecuteNonQueryAsync();
        var insertQuery = "INSERT INTO category (Name) VALUES (@name) RETURNING Id;";

        await using var insertCommand = new NpgsqlCommand(insertQuery, connection);
        insertCommand.Parameters.AddWithValue("@name", testCategoryName);

        var generatedId = await insertCommand.ExecuteScalarAsync();

        var updateQuery = "UPDATE category SET name = @name  WHERE id = @id;";

        await using var updateCommand = new NpgsqlCommand(updateQuery, connection);
        updateCommand.Parameters.AddWithValue("@name", testCategoryUpdateName);
        updateCommand.Parameters.AddWithValue("@id", generatedId);
        await updateCommand.ExecuteNonQueryAsync();
        var selectQuery = "SELECT Name FROM category WHERE Id = @id;";
        await using var selectCommand = new NpgsqlCommand(selectQuery, connection);
        selectCommand.Parameters.AddWithValue("@id", generatedId);

        var retrievedName = (string?)await selectCommand.ExecuteScalarAsync();
        var repository = new CategoryRepository(db, neo4j);
        var dto = new CategoryDto
        {
            Id = (int)generatedId,
            Name = testCategoryUpdateName
        };
        await repository.UpdateCategory(dto);
        var selectRepQuery = "SELECT Name FROM category WHERE Id = @id;";
        await using var selectRepCommand = new NpgsqlCommand(selectRepQuery, connection);
        selectRepCommand.Parameters.AddWithValue("@id", generatedId);
        var retrievedRepName = (string?)await selectRepCommand.ExecuteScalarAsync();

        Assert.NotNull(retrievedName);
        Assert.Equal(testCategoryUpdateName, retrievedName);
        Assert.Equal(testCategoryUpdateName, retrievedRepName);
    }

    [Fact]
    public async Task DeleteCategory_ShouldSaveAndRetrieveCorrectData()
    {
        var connectionString = _fixture.Postgres.GetConnectionString();
        var connectionNeo4jString = _fixture.Neo4j.GetConnectionString();
        var db = new DatabaseConnectie(connectionString);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Neo4j:Uri"] = _fixture.Neo4j.GetConnectionString(),
                ["Neo4j:User"] = "neo4j",
                ["Neo4j:Password"] = "password"
            })
            .Build();

        var neo4j = new Neo4jService(config);
        var testCategoryName = "Elektronica";

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        await using var cleanupCommand =
            new NpgsqlCommand("DELETE FROM category;", connection);
        await cleanupCommand.ExecuteNonQueryAsync();
        var insertQuery = "INSERT INTO category (Name) VALUES (@name) RETURNING Id;";

        await using var insertCommand = new NpgsqlCommand(insertQuery, connection);
        insertCommand.Parameters.AddWithValue("@name", testCategoryName);

        var generatedId = await insertCommand.ExecuteScalarAsync();
        var sql = "DELETE FROM category WHERE id = @id;";
        await using var deleteCommand = new NpgsqlCommand(sql, connection);
        deleteCommand.Parameters.AddWithValue("@id", generatedId);
        await deleteCommand.ExecuteNonQueryAsync();
        var selectQuery = "SELECT Name FROM category WHERE Id = @id;";
        await using var selectCommand = new NpgsqlCommand(selectQuery, connection);
        selectCommand.Parameters.AddWithValue("@id", generatedId);

        var retrievedName = (string?)await selectCommand.ExecuteScalarAsync();

        var repository = new CategoryRepository(db, neo4j);
        var dto = new CategoryDto
        {
            Id = (int)generatedId + 1,
            Name = "Electricity2"
        };
        await repository.AddCategory(dto);

        await repository.DeleteCategory(dto.Id);

        var selectRepQuery = "SELECT Name FROM category WHERE Id = @id;";
        await using var selectRepCommand = new NpgsqlCommand(selectRepQuery, connection);
        selectRepCommand.Parameters.AddWithValue("@id", dto.Id);
        var retrievedRepName = (string?)await selectRepCommand.ExecuteScalarAsync();
        Assert.Null(retrievedName);
        Assert.Null(retrievedRepName);
    }
}
