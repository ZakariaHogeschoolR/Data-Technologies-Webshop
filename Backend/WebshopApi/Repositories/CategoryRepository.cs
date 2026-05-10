using ApplicationDbContext;

using DataTransferObject;

using models;

using Npgsql;

public class CategoryRepository
{
    private readonly DatabaseConnectie _dbConnectie;
    private readonly Neo4jService _neo4j;

    public CategoryRepository(DatabaseConnectie dbConnectie, Neo4jService neo4j)
    {
        _dbConnectie = dbConnectie;
        _neo4j = neo4j;
    }

    public async Task<List<Categories>> GetAllCategories()
    {
        var categories = new List<Categories>();
        using var conn = await _dbConnectie.GetConnection();
        var sql = "SELECT * FROM category;";

        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            categories.Add(new Categories
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
            });
        }
        // only activate when there is already items in the database
        //await GetAllCategoriesForGraph();
        return categories;
    }

    public async Task<List<Categories>> GetAllCategoriesForGraph()
    {
        // this is method should only be used once everytime we delete the entity relational diagram.
        // and even than it should still not be used cause addproduct does the same thing but just for one singular category.
        // this method is only usefull if there are already categories in the database.
        var categories = new List<Categories>();
        using var conn = await _dbConnectie.GetConnection();
        var sql = "SELECT * FROM category;";

        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            categories.Add(new Categories
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
            });
        }

        foreach (Categories category in categories)
        {
            // it takes the items for in the graph database
            await AddCategoryToGraph(category);
        }

        return categories;
    }

    public async Task<Categories?> GetCategoryById(int id)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = "SELECT * FROM category WHERE id = @id ";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Categories
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
            };
        }

        return null;
    }

    public async Task<List<Categories?>> GetCategoryByPrice(double price)
    {
        var categories = new List<Categories>();
        using var conn = await _dbConnectie.GetConnection();

        var sql = "SELECT * FROM category WHERE Categorys.Price = @price";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@price", price);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            categories.Add(new Categories
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
            });
        }
        return categories;
    }

    public async Task<List<Categories?>> GetCategoryByTeam(string name)
    {
        var categories = new List<Categories>();
        using var conn = await _dbConnectie.GetConnection();

        var sql = "SELECT * FROM Category WHERE Categorys.Name = @name";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@name", name);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            categories.Add(new Categories
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
            });
        }

        return categories;
    }

    public async Task<List<Categories?>> GetCategorysByPrice(double price)
    {
        var categories = new List<Categories>();
        using var conn = await _dbConnectie.GetConnection();
        var sql = "SELECT * FROM Categorys WHERE price >= @price";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@price", price);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            categories.Add(new Categories
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
            });
        }

        return categories;
    }

    public async Task AddCategory(CategoryDto category)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = @"INSERT INTO category (name)
                    VALUES (@name)";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@name", category.Name);
        await cmd.ExecuteNonQueryAsync();
        await AddCategoryToGraph(category);
    }

    private async Task AddCategoryToGraph(CategoryDto category)
    {
        await using var session = _neo4j.CreateSession();

        var query = @"
            MERGE (c:Category {id: $id})
            SET c.name = $name
        ";

        await session.RunAsync(query, new
        {
            id = category.Id,
            name = category.Name,
        });
    }

    private async Task AddCategoryToGraph(Categories category)
    {
        await using var session = _neo4j.CreateSession();

        var query = @"
            MERGE (c:Category {id: $id})
            SET c.name = $name
        ";

        await session.RunAsync(query, new
        {
            id = category.Id,
            name = category.Name,
        });
    }

    public async Task UpdateCategory(CategoryDto category)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = @"UPDATE category
                    SET name = @name WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", category.Id);
        cmd.Parameters.AddWithValue("@name", category.Name);

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteCategory(int id)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = "DELETE FROM category WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}
