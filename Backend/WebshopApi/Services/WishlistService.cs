using models;
namespace Service
{
    public class WishlistService
    {
        private readonly WishlistRepository _wishlistRepository;
        public WishlistService(WishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }
        public async Task<List<Wishlists?>> GetAllWishLists(){}
        public async Task<Wishlists?> GetWishlistsById(){}
        public async Task<List<Products?>> GetAllProducts(){}
        public async Task<Products?> GetProductsById(){}
        public async void CreateService(){}
        public async void UpdateService(){}
        public async void DeleteService(){}
    }
}