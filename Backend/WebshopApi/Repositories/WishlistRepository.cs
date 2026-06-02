using ApplicationDbContext;

using DataTransferObject;

using models;

using Npgsql;

public class WishlistRepository: IWishlist
{
    private readonly DatabaseConnectie _dbconnectie;

    public WishlistRepository(DatabaseConnectie dbconnectie)
    {
        _dbconnectie = dbconnectie;
    }
    public async Task<List<Wishlists?>> GetAllWishLists()
    {
        List<Wishlists?> wishlists = new List<Wishlists?>();
        using var conn = await _dbconnectie.GetConnection();
        var sql = "SELECT * FROM wishlist";
        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            int productIdOrdinal = reader.GetOrdinal("product_id");
            wishlists.Add(new Wishlists
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Userid = reader.GetInt32(reader.GetOrdinal("user_id")),
                Productid = reader.IsDBNull(productIdOrdinal) ? null : reader.GetInt32(reader.GetOrdinal("product_id"))
            });
        }
        return wishlists;
    }
    public async Task<List<Wishlists?>> GetWishlistsById(int id)
    {
        using var conn = await _dbconnectie.GetConnection();
        var listofwishlists = new List<Wishlists?>();
        var sql = @"SELECT * FROM wishlist
        WHERE name= (SELECT name FROM wishlist WHERE id= @id)
        AND user_id= (SELECT user_id FROM wishlist WHERE id =@id)";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@user_id", id);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            int productIdOrdinal = reader.GetOrdinal("product_id");
            listofwishlists.Add(
                new Wishlists
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Productid = reader.IsDBNull(productIdOrdinal) ? null : reader.GetInt32(reader.GetOrdinal("product_id")),
                    // Productid = reader.GetInt32(reader.GetOrdinal("product_id")),
                    Userid = reader.GetInt32(reader.GetOrdinal("user_id"))
                }
            );
        }
        return listofwishlists;
    }
    public async Task<List<Wishlists?>> GetWishlistsByUserId(int id)
    {
        using var conn = await _dbconnectie.GetConnection();
        var listofwishlists = new List<Wishlists?>();
        var sql = @"SELECT * FROM wishlist WHERE user_id= @user_id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@user_id", id);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            int productIdOrdinal = reader.GetOrdinal("product_id");
            listofwishlists.Add(
                new Wishlists
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Productid = reader.IsDBNull(productIdOrdinal) ? null : reader.GetInt32(reader.GetOrdinal("product_id")),
                    // Productid = reader.GetInt32(reader.GetOrdinal("product_id")),
                    Userid = reader.GetInt32(reader.GetOrdinal("user_id"))
                }
            );
        }
        return listofwishlists;
    }
    public async Task<Wishlists?> AddWishlist(WishlistDTO wishlistDTO)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "INSERT INTO wishlist (name, product_id, user_id) VALUES(@name,@productid,@userid) RETURNING id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@name", wishlistDTO.Name);
        cmd.Parameters.AddWithValue("@productid", (object)wishlistDTO.ProductId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@userid", wishlistDTO.UserId);
        var id = (int)await cmd.ExecuteScalarAsync();
        return new Wishlists()
        {
            Id = id,
            Userid = wishlistDTO.UserId,
            Name = wishlistDTO.Name,
            Productid = wishlistDTO.ProductId,
            CreatedAt = wishlistDTO.CreatedAt,
            UpdatedAt = wishlistDTO.UpdatedAt
        };
    }
    public async Task UpdateWishlist(WishlistDTO wishlistDTO)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "UPDATE wishlist SET name= @name, product_id= @productid, user_id= @userid WHERE id= @id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", wishlistDTO.Id);
        cmd.Parameters.AddWithValue("@name", wishlistDTO.Name);
        cmd.Parameters.AddWithValue("@productid", (object)wishlistDTO.ProductId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@userid", wishlistDTO.UserId);
        await cmd.ExecuteNonQueryAsync();
    }
    public async Task DeleteWishlist(int id)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "DELETE FROM wishlist WHERE id = @id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();
    }
    public async Task DeleteProduct(int id, string name)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = @"UPDATE wishlist SET product_id= NULL
        WHERE product_id= @pid AND name= @name";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@pid", id);
        cmd.Parameters.AddWithValue("@name", name);
        await cmd.ExecuteNonQueryAsync();
    }
}
