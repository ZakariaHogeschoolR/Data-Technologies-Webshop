using ApplicationDbContext;

using DataTransferObject;

using models;

using Npgsql;

public class ProductRepository
{
    private readonly DatabaseConnectie _dbConnectie;
    private readonly Neo4jService _neo4j;

    public ProductRepository(DatabaseConnectie dbConnectie, Neo4jService neo4j)
    {
        _dbConnectie = dbConnectie;
        _neo4j = neo4j;
    }

    public async Task<List<Products>> GetAllProducts()
    {
        var products = new List<Products>();
        using var conn = await _dbConnectie.GetConnection();
        var sql = "SELECT * FROM products LIMIT 30;";

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
        // only activate when there is already items in the database
        //await GetAllProductsForGraph();
        return products;
    }

    public async Task<List<Products>> GetAllProductsForGraph()
    {
        // this is method should only be used once everytime we delete the entity relational diagram.
        // and even than it should still not be used cause addproduct does the same thing but just for one singular product.
        // this method is only usefull if there are already products in the database.
        var products = new List<Products>();
        using var conn = await _dbConnectie.GetConnection();
        var sql = "SELECT * FROM products;";

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

        foreach (Products product in products)
        {
            // it takes the items for in the graph database
            await AddProductToGraph(product);
        }

        return products;
    }

    public async Task<List<Products>> GetAllProductsAdmin()
    {
        var products = new List<Products>();
        using var conn = await _dbConnectie.GetConnection();
        var sql = "SELECT * FROM products;";

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

    public async Task<List<Products>> GetAllProductsAdminPaged(int page, int pageSize)
    {
        var products = new List<Products>();
        using var conn = await _dbConnectie.GetConnection();
        var sql = "SELECT * FROM products ORDER BY id LIMIT @pageSize OFFSET @offset;";
        var offset = (page - 1) * pageSize;

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@pageSize", pageSize);
        cmd.Parameters.AddWithValue("@offset", offset);
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

    public async Task<List<Products>> GetProductsPrev(int lastId)
    {
        var products = new List<Products>();
        using var conn = await _dbConnectie.GetConnection();

        var sql = "SELECT * FROM products WHERE id < @lastId ORDER BY id DESC LIMIT 30";

        using var cmd = new NpgsqlCommand(sql, conn);
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
    public async Task<List<Products>> GetProductsNext(int lastId)
    {
        var products = new List<Products>();
        using var conn = await _dbConnectie.GetConnection();

        var sql = "SELECT * FROM products WHERE id > @lastId ORDER BY id LIMIT 30;";

        using var cmd = new NpgsqlCommand(sql, conn);
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

    public async Task<List<Products>> SearchProductsByName(string name)
    {
        var products = new List<Products>();
        using var conn = await _dbConnectie.GetConnection();

        var sql = "SELECT * FROM products WHERE LOWER(name) LIKE LOWER('%' || @name || '%') LIMIT 5";
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

    public async Task<Products?> GetProductById(int id)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = "SELECT * FROM products WHERE id = @id ";

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

        var sql = "SELECT * FROM products WHERE LOWER(name) LIKE LOWER(@name)";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@name", $"%{name}%");

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

    public async Task<List<Products?>> GetProductsByPrice(double price)
    {
        var products = new List<Products>();
        using var conn = await _dbConnectie.GetConnection();
        var sql = "SELECT * FROM products WHERE price >= @price";

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

    public async Task<List<Products>> GetProductsByCategories(List<int> categoryIds, int page, int pageSize)
    {
        var products = new List<Products>();
        using var conn = await _dbConnectie.GetConnection();
        
        var sql = @"SELECT p.* FROM products p
                    INNER JOIN product_categories pc ON p.id = pc.product_id
                    WHERE pc.category_id = ANY(@categoryIds)
                    ORDER BY p.id
                    LIMIT @pageSize OFFSET @offset";

        var offset = (page - 1) * pageSize;

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@categoryIds", categoryIds.ToArray());
        cmd.Parameters.AddWithValue("@pageSize", pageSize);
        cmd.Parameters.AddWithValue("@offset", offset);

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
        await AddProductToGraph(product);
    }

    private async Task AddProductToGraph(ProductDto product)
    {
        await using var session = _neo4j.CreateSession();

        var query = @"
            MERGE (p:Product {id: $id})
            SET p.name = $name,
                p.description = $description,
                p.price = $price,
                p.productImage = $image

            MERGE (t:Team {id: $teamId})
            MERGE (p)-[:BELONGS_TO]->(t)
        ";

        await session.RunAsync(query, new
        {
            id = product.Id,
            name = product.Name,
            description = product.Description,
            price = product.Price,
            image = product.ProductImage,
            teamId = product.TeamId
        });
    }

    private async Task AddProductToGraph(Products product)
    {
        await using var session = _neo4j.CreateSession();

        var query = @"
            MERGE (p:Product {id: $id})
            SET p.name = $name,
                p.description = $description,
                p.price = $price,
                p.productImage = $image

            MERGE (t:Team {id: $teamId})
            MERGE (p)-[:BELONGS_TO]->(t)
        ";

        await session.RunAsync(query, new
        {
            id = product.Id,
            name = product.Name,
            description = product.Description,
            price = product.Price,
            image = product.ProductImage,
            teamId = product.TeamId
        });
    }

    public async Task<int> AddProductScrape(ProductDto product)
    {
        if (!string.IsNullOrWhiteSpace(product.ProductImage))
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
        return -1;
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
