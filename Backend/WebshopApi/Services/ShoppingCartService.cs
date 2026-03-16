using models;

namespace Service
{
    public class ShoppingCartService
    {
        private readonly ShoppingCartRepository _shoppingcartRepository;
        public ShoppingCartService(ShoppingCartRepository shoppingcartRepository)
        {
            _shoppingcartRepository = shoppingcartRepository;        
        }
        public async Task<List<ShoppingCarts?>> GetAllShoppingCarts()
        {
            throw new NotImplementedException();
        }
        public async Task<ShoppingCarts?> GetShoppingCartById()
        {
            throw new NotImplementedException();
        }
        public async Task<ShoppingCarts?> AddShoppingcarts()
        {
            throw new NotImplementedException();
        }
        public async void CreateService(Products product){}
        public async void UpdateteService(){}
        public async void DeleteService(){}
        public async void DeleteProductsService(){}
    }
}