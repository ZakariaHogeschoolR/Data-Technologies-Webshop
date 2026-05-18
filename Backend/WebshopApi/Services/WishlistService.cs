using DataTransferObject;

using models;
namespace Service;

public class WishlistService
{
    private readonly WishlistRepository _wishlistRepository;
    public WishlistService(WishlistRepository wishlistRepository)
    {
        _wishlistRepository = wishlistRepository;
    }
    public async Task<List<Wishlists?>> GetAllWishLists()
    {
        // throw new NotImplementedException();
        Task<List<Wishlists?>> wishlists = _wishlistRepository.GetAllWishLists();
        return await wishlists;
    }
    public async Task<List<Wishlists?>> GetWishlistsById(int id)
    {
        // throw new NotImplementedException();
        Task<List<Wishlists?>> wishlist = _wishlistRepository.GetWishlistsById(id);
        return await wishlist;
    }
    public async Task<List<Wishlists?>> GetWishlistsByUserId(int id)
    {
        // throw new NotImplementedException();
        Task<List<Wishlists?>> wishlist = _wishlistRepository.GetWishlistsByUserId(id);
        return await wishlist;
    }
    public async Task<Products?> GetProductsById()
    {
        throw new NotImplementedException();
    }
    public async Task<Wishlists> CreateService(WishlistDTO wishlistDTO)
    {
        return await _wishlistRepository.AddWishlist(wishlistDTO);
    }
    public async Task UpdateService(WishlistDTO wishlistDTO)
    {
        await _wishlistRepository.UpdateWishlist(wishlistDTO);
    }
    public async Task DeleteService(int id)
    {
        await _wishlistRepository.DeleteWishlist(id);
    }
    public async Task DeleteProduct(int id, string name)
    {
        await _wishlistRepository.DeleteProduct(id, name);
    }
}
