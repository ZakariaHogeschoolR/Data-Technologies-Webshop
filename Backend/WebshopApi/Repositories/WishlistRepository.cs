using ApplicationDbContext;
using models;

public class WishlistRepository
{
    private readonly DatabaseConnectie _dbconnectie;

    public WishlistRepository(DatabaseConnectie dbconnectie)
    {
        _dbconnectie = dbconnectie;
    }
    public async Task<List<Wishlists?>> GetAllWishLists()
    {
        throw new NotImplementedException();
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