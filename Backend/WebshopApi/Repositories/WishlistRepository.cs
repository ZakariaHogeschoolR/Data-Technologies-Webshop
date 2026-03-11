using ApplicationDbContext;
using models;

public class WishlistRepository
{
    private readonly DatabaseConnectie _dbconnectie;

    public WishlistRepository(DatabaseConnectie dbconnectie)
    {
        _dbconnectie = dbconnectie;
    }
    public async Task<List<Wishlists?>> GetAllWishLists(){}
    public async Task<Wishlists?> GetWishlistsById(){}
    public async Task<List<Products?>> GetAllProducts(){}
    public async Task<Products?> GetProductsById(){}
    public async void AddWishlist(){}
    public async void UpdateWishlist(){}
    public async void DeleteWishlist(){}
}