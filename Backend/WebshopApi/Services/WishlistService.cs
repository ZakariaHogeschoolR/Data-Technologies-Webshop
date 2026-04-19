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
        var wishlists = _wishlistRepository.GetAllWishLists();
        return await wishlists;
    }

    public async Task<Wishlists?> GetWishlistsById(int id)
    {
        // throw new NotImplementedException();
        var wishlist = _wishlistRepository.GetWishlistsById(id);
        return await wishlist;
    }

    public async Task<List<Products?>> GetAllProducts() => throw new NotImplementedException();

    public async Task<Products?> GetProductsById() => throw new NotImplementedException();

    public async void CreateService(WishlistDto wishlistDTO) => _wishlistRepository.AddWishlist(wishlistDTO);

    public async void UpdateService(WishlistDto wishlistDTO) => _wishlistRepository.UpdateWishlist(wishlistDTO);

    public async void DeleteService(int id) => _wishlistRepository.DeleteWishlist(id);
}
