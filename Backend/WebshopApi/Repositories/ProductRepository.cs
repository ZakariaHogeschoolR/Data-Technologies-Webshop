using Npgsql;
using System.Threading.Tasks;
using models;
using System.Data.SqlTypes;
using System.Data.Common;
using System.Reflection.Metadata;
using ApplicationDbContext;

public class ProductRepository
{
    private readonly DatabaseConnectie _dbConnectie;

    public ProductRepository(DatabaseConnectie dbConnectie)
    {
        _dbConnectie = dbConnectie;
    }

    public async Task<List<Products?>> GetAllProducts()
    {
        var products = new List<Products>();
        using var conn = _dbConnectie.GetConnection();
        var sql = "SELECT * FROM products";

        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while(await reader.ReadAsync())
        {
            products.Add( new Products
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Description = reader.GetString(reader.GetOrdinal("description"))
            });
        }

        return products;
    }

    public async Task<Products?> GetProductById(int id)
    {
        using var conn = _dbConnectie.GetConnection();

        var sql = "SELECT * FROM products WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Products
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Description = reader.GetString(reader.GetOrdinal("description"))
            };
        }

        return null;
    }

    public async void AddProduct(Products product)
    {
        using var conn = _dbConnectie.GetConnection();

        var sql = "INSERT INTO products (name, description) VALUES (@name, @description)";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@name", product.Name);
        cmd.Parameters.AddWithValue("@description", product.Description);
        await cmd.ExecuteNonQueryAsync();
    }

    public async void UpdateProduct(Products product)
    {
        using var conn = _dbConnectie.GetConnection();

        var sql = "UPDATE products SET name = @name, description = @description WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", product.Id);
        cmd.Parameters.AddWithValue("@name", product.Name);
        cmd.Parameters.AddWithValue("@description", product.Description);
        await cmd.ExecuteNonQueryAsync();
    }

    public async void DeleteProduct(int id)
    {
        using var conn = _dbConnectie.GetConnection();

        var sql = "DELETE FROM products WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}