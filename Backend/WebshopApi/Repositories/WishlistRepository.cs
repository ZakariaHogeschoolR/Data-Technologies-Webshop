using ApplicationDbContext;

using DataTransferObject;

using models;

using Npgsql;

public class WishlistRepository
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
        var sql = "SELECT * FROM Wishlist";
        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            wishlists.Add(new Wishlists
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Userid = reader.GetInt32(reader.GetOrdinal("user_id")),
                Productid = reader.GetInt32(reader.GetOrdinal("user_id"))
            });
        }
        return wishlists;
    }
    public async Task<Wishlists?> GetWishlistsById(int id)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "SELECT * FROM Wishlist WHERE (id) VALUES (@id)";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            return new Wishlists
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Productid = reader.GetInt32(reader.GetOrdinal("product_id")),
                Userid = reader.GetInt32(reader.GetOrdinal("user_id"))
            };
        }
        return null;
    }
    public async Task<List<Products?>> GetAllProducts()
    {
        throw new NotImplementedException();
    }
    public async Task<Products?> GetProductsById()
    {
        throw new NotImplementedException();
    }
    public async void AddWishlist(WishlistDTO wishlistDTO)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "INSERT INTO wishlist (name, product_id, user_id) VALUES(@name,@productid,@userid)";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@name", wishlistDTO.Name);
        cmd.Parameters.AddWithValue("@productid", wishlistDTO.ProductId);
        cmd.Parameters.AddWithValue("@userid", wishlistDTO.UserId);
        using var reader = cmd.ExecuteNonQueryAsync();
    }
    public async void UpdateWishlist(WishlistDTO wishlistDTO)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "UPDATE Wishlist SET (name, product_id, user_id) VALUES(@name,@productid,@userid) WHERE id= @id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", wishlistDTO.Id);
        cmd.Parameters.AddWithValue("@name", wishlistDTO.Name);
        cmd.Parameters.AddWithValue("@productid", wishlistDTO.ProductId);
        cmd.Parameters.AddWithValue("@userid", wishlistDTO.UserId);
    }
    public async void DeleteWishlist(int id)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "DELETE * FROM Wishlist WHERE (id) VALUES(id)";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
    }
}
