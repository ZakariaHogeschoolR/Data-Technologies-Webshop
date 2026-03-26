using Npgsql;
using models;
using ApplicationDbContext;
using DataTransferObject;

public class ProductRepository
{
    private readonly DatabaseConnectie _dbConnectie;

    public ProductRepository(DatabaseConnectie dbConnectie)
    {
        _dbConnectie = dbConnectie;
    }

    public async Task<List<Products>> GetAllProducts()
    {
        var products = new List<Products>();
        using var conn = await _dbConnectie.GetConnection();
        var sql = "SELECT * FROM products";

        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            products.Add(new Products
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                ProductImage = reader.GetString(reader.GetOrdinal("product_image")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Description = reader.GetString(reader.GetOrdinal("description")),
                Price = reader.GetDecimal(reader.GetOrdinal("price"))
            });
        }

        return products;
    }

    public async Task<Products?> GetProductById(int id)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = "SELECT * FROM products WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Products
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                ProductImage = reader.GetString(reader.GetOrdinal("product_image")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Description = reader.GetString(reader.GetOrdinal("description")),
                Price = reader.GetDecimal(reader.GetOrdinal("price"))
            };
        }

        return null;
    }

    public async Task<List<Products?>> GetProductByPrice(double price)
    {
        var products = new List<Products>();
        using var conn = await _dbConnectie.GetConnection();

        var sql = "SELECT * FROM products WHERE products.Price = @price";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@price", price);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            products.Add(new Products
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                ProductImage = reader.GetString(reader.GetOrdinal("product_image")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Description = reader.GetString(reader.GetOrdinal("description")),
                Price = reader.GetDecimal(reader.GetOrdinal("price"))
            });
        }

        return products;
    }

    public async Task<List<Products?>> GetProductByName(string name)
    {
        var products = new List<Products>();
        using var conn = await _dbConnectie.GetConnection();

        var sql = "SELECT * FROM products WHERE products.Name = @name";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@name", name);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            products.Add(new Products
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                ProductImage = reader.GetString(reader.GetOrdinal("product_image")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Description = reader.GetString(reader.GetOrdinal("description")),
                Price = reader.GetDecimal(reader.GetOrdinal("price"))
            });
        }

        return products;
    }

    public async Task AddProduct(ProductDto product)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = @"INSERT INTO products (product_image, name, description, price)
                    VALUES (@productImage, @name, @description, @price)";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@productImage", product.ProductImage);
        cmd.Parameters.AddWithValue("@name", product.Name);
        cmd.Parameters.AddWithValue("@description", product.Description);
        cmd.Parameters.AddWithValue("@price", product.Price);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateProduct(ProductDto product)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = @"UPDATE products
                    SET product_image = @productImage,
                        name = @name,
                        description = @description,
                        price = @price
                    WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", product.Id);
        cmd.Parameters.AddWithValue("@productImage", product.ProductImage);
        cmd.Parameters.AddWithValue("@name", product.Name);
        cmd.Parameters.AddWithValue("@description", product.Description);
        cmd.Parameters.AddWithValue("@price", product.Price);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteProduct(int id)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = "DELETE FROM products WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}