using DataTransferObject;

using models;

public interface IWishlistService
{
    Task<List<Wishlists?>> GetAllWishLists();
    Task<List<Wishlists?>> GetWishlistsById(int id);
    Task<List<Wishlists?>> GetWishlistsByUserId(int id);
    Task<Products?> GetProductsById();
    Task<Wishlists> CreateService(WishlistDTO wishlistDTO);
    Task UpdateService(WishlistDTO wishlistDTO);
    Task DeleteService(int id);
    Task DeleteProduct(int id, string name);
}
