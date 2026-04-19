using Npgsql;
using models;
using ApplicationDbContext;
using DataTransferObject;

public class ProductCategoryRepository
{
    private readonly DatabaseConnectie _dbConnectie;

    public ProductCategoryRepository(DatabaseConnectie dbConnectie)
    {
        _dbConnectie = dbConnectie;
    }

    public async Task<List<Products>> GetAllProductCategorys(int id)
    {
        var products = new List<Products>();

        using var conn = await _dbConnectie.GetConnection();

        var sql = @"
            SELECT p.*
            FROM Products p
            INNER JOIN product_categories cp ON p.Id = cp.product_id
            WHERE cp.category_id = @categoryId
            LIMIT 30";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("categoryId", id);

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            products.Add(new Products
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                ProductImage = reader.GetString(reader.GetOrdinal("product_image")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Description = reader.GetString(reader.GetOrdinal("description")),
                Price = reader.GetDecimal(reader.GetOrdinal("price")),
                TeamId = reader.GetInt32(reader.GetOrdinal("team_id"))
            });
        }

        return products;
    }

    public async Task<List<Products>> GetProductsPrev(int categoryId, int lastId)
    {
        var products = new List<Products>();
        using var conn = await _dbConnectie.GetConnection();

        var sql = @"
            SELECT p.*
            FROM products p
            INNER JOIN product_categories pc ON p.id = pc.product_id
            WHERE pc.category_id = @categoryId
            AND p.id < @lastId
            ORDER BY p.id DESC
            LIMIT 30";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("categoryId", categoryId);
        cmd.Parameters.AddWithValue("lastId", lastId);

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            products.Add(new Products
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                ProductImage = reader.GetString(reader.GetOrdinal("product_image")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Description = reader.GetString(reader.GetOrdinal("description")),
                Price = reader.GetDecimal(reader.GetOrdinal("price")),
                TeamId = reader.GetInt32(reader.GetOrdinal("team_id"))
            });
        }

        products.Reverse();
        return products;
    }

    public async Task<List<Products>> GetProductsNext(int categoryId, int lastId)
    {
        var products = new List<Products>();
        using var conn = await _dbConnectie.GetConnection();

        var sql = @"
            SELECT p.*
            FROM products p
            INNER JOIN product_categories pc ON p.id = pc.product_id
            WHERE pc.category_id = @categoryId
            AND p.id > @lastId
            ORDER BY p.id
            LIMIT 30";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("categoryId", categoryId);
        cmd.Parameters.AddWithValue("lastId", lastId);

        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            products.Add(new Products
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                ProductImage = reader.GetString(reader.GetOrdinal("product_image")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Description = reader.GetString(reader.GetOrdinal("description")),
                Price = reader.GetDecimal(reader.GetOrdinal("price")),
                TeamId = reader.GetInt32(reader.GetOrdinal("team_id"))
            });
        }

        return products;
    }
}