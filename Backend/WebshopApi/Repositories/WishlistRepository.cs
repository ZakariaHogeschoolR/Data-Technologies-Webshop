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
        var sql = "SELECT * FROM wishlist";
        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            wishlists.Add(new Wishlists
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
                Userid = reader.GetInt32(reader.GetOrdinal("user_id")),
                Productid = reader.GetInt32(reader.GetOrdinal("product_id"))
            });
        }
        return wishlists;
    }
    public async Task<List<Wishlists?>> GetWishlistsById(int id)
    {
        using var conn = await _dbconnectie.GetConnection();
        var listofwishlists = new List<Wishlists>();
        var sql = "SELECT * FROM wishlist WHERE user_id= @user_id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@user_id", id);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            listofwishlists.Add(
                new Wishlists
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Productid = reader.GetInt32(reader.GetOrdinal("product_id")),
                    Userid = reader.GetInt32(reader.GetOrdinal("user_id"))
                }
            );
        }
        return listofwishlists;
    }
    public async Task<List<Products?>> GetAllProducts()
    {
        throw new NotImplementedException();
    }
    public async Task<Products?> GetProductsById()
    {
        throw new NotImplementedException();
    }
    public async Task<Wishlists?> AddWishlist(WishlistDTO wishlistDTO)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "INSERT INTO wishlist (name, product_id, user_id) VALUES(@name,@productid,@userid) RETURNING id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@name", wishlistDTO.Name);
        cmd.Parameters.AddWithValue("@productid", wishlistDTO.ProductId);
        cmd.Parameters.AddWithValue("@userid", wishlistDTO.UserId);
        var id = (int)await cmd.ExecuteScalarAsync();
        return new Wishlists()
        {
            Id = id,
            Userid = wishlistDTO.UserId,
            Name = wishlistDTO.Name,
            Productid = wishlistDTO.ProductId
        };
    }
    public async void UpdateWishlist(WishlistDTO wishlistDTO)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "UPDATE wishlist SET (name, product_id, user_id) VALUES(@name,@productid,@userid) WHERE user_id= @id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", wishlistDTO.Id);
        cmd.Parameters.AddWithValue("@name", wishlistDTO.Name);
        cmd.Parameters.AddWithValue("@productid", wishlistDTO.ProductId);
        cmd.Parameters.AddWithValue("@userid", wishlistDTO.UserId);
    }
    public async void DeleteWishlist(int id)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "DELETE * FROM wishlist WHERE (id) VALUES(id)";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
    }
}
