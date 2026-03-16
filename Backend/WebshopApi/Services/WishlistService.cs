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
        public async void CreateService(){}
        public async void UpdateService(){}
        public async void DeleteService(){}
    }
}