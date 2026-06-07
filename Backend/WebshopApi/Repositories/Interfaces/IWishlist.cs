using DataTransferObject;

using models;

public interface IWishlist
{
    Task<List<Wishlists?>> GetAllWishLists();

    Task<List<Wishlists?>> GetWishlistsById(int id);

    Task<List<Wishlists?>> GetWishlistsByUserId(int id);

    Task<Wishlists?> AddWishlist(WishlistDTO wishlistDTO);

    Task UpdateWishlist(WishlistDTO wishlistDTO);

    Task DeleteWishlist(int id);

    Task DeleteProduct(int id, string name);

}
