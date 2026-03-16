using ApplicationDbContext;
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
        while(await reader.ReadAsync())
        {
            wishlists.Add(new Wishlists{});
        }
        return wishlists;
    }
    public async Task<Wishlists?> GetWishlistsById()
    {
        throw new NotImplementedException();
    }
    public async Task<List<Products?>> GetAllProducts()
    {
        throw new NotImplementedException();
    }
    public async Task<Products?> GetProductsById()
    {
        throw new NotImplementedException();
    }
    public async void AddWishlist(){}
    public async void UpdateWishlist(){}
    public async void DeleteWishlist(){}
}