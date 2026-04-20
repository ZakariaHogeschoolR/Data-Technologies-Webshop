using ApplicationDbContext;

using DataTransferObject;

using models;

using Npgsql;

public class CategoryRepository
{
    private readonly DatabaseConnectie _dbConnectie;

    public CategoryRepository(DatabaseConnectie dbConnectie)
    {
        _dbConnectie = dbConnectie;
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
