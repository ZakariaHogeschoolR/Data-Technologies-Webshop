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
                Price = reader.GetDecimal(reader.GetOrdinal("price")),
                TeamId = reader.GetInt32(reader.GetOrdinal("team_id"))
            });
        }

        return products;
    }
    
    public async Task<List<Products>> GetAllProductsByTeam(int id)
    {
        var products = new List<Products>();
        using var conn = await _dbConnectie.GetConnection();
        var sql = "SELECT * FROM products WHERE team_id = @Id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Id", id);
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
                Price = reader.GetDecimal(reader.GetOrdinal("price")),
                TeamId = reader.GetInt32(reader.GetOrdinal("team_id"))
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

    public async Task<List<Products?>> GetProductsByPrice(double price)
    {
        var products = new List<Products>();
        using var conn = await _dbConnectie.GetConnection();
        var sql = "SELECT * FROM products WHERE price >= @price";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@price", price);
        using var reader = await cmd.ExecuteReaderAsync();

        while(await reader.ReadAsync())
        {
            products.Add( new Products
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

        var sql = @"INSERT INTO products (product_image, name, description, price, team_id)
                    VALUES (@productImage, @name, @description, @price, @teamId)";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@productImage", product.ProductImage);
        cmd.Parameters.AddWithValue("@name", product.Name);
        cmd.Parameters.AddWithValue("@description", product.Description);
        cmd.Parameters.AddWithValue("@price", product.Price);
        cmd.Parameters.AddWithValue("@teamId", product.TeamId);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<int> AddProductScrape(ProductDto product)
    {
        using var conn = await _dbConnectie.GetConnection();
        var sql = @"
            INSERT INTO products (product_image, name, description, price, team_id)
            VALUES (@productImage, @name, @description, @price, @teamId)
            RETURNING id;
        ";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@productImage", product.ProductImage);
        cmd.Parameters.AddWithValue("@name", product.Name);
        cmd.Parameters.AddWithValue("@description", product.Description);
        cmd.Parameters.AddWithValue("@price", product.Price);
        cmd.Parameters.AddWithValue("@teamId", product.TeamId);

        var productId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
        return productId;
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

    public async Task<int> GetOrCreateTeam(string name, string type)
    {
        using var conn = await _dbConnectie.GetConnection();
        var sql = @"
            INSERT INTO teams (name, type)
            VALUES (@name, @type)
            ON CONFLICT (name)
            DO UPDATE SET type = EXCLUDED.type
            RETURNING id;
        ";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@type", type);

        var result = await cmd.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task<int> GetOrCreateCategory(string name)
    {
        using var conn = await _dbConnectie.GetConnection();
        var sql = @"
            INSERT INTO category (name)
            VALUES (@name)
            ON CONFLICT (name)
            DO UPDATE SET name = EXCLUDED.name
            RETURNING id;
        ";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@name", name);

        var result = await cmd.ExecuteScalarAsync();

        return Convert.ToInt32(result);
    }

    public async Task AddProductCategory(int productId, int categoryId)
    {
        using var conn = await _dbConnectie.GetConnection();
        var sql = @"
            INSERT INTO product_categories (product_id, category_id)
            VALUES (@productId, @categoryId)
            ON CONFLICT (product_id, category_id) DO NOTHING;
        ";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@productId", productId);
        cmd.Parameters.AddWithValue("@categoryId", categoryId);

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