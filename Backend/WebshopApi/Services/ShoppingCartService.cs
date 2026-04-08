using DataTransferObject;
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
            // throw new NotImplementedException();
            Task<List<ShoppingCarts?>> shoppingcarts =  _shoppingcartRepository.GetAllShoppingCarts();
            return await shoppingcarts;
        }
        public async Task<List<ShoppingCarts?>> GetShoppingCartById(int id)
        {
            // throw new NotImplementedException();
            Task<List<ShoppingCarts?>> shoppingCarts = _shoppingcartRepository.GetShoppingCartById(id);
            return await shoppingCarts;
        }
        public async Task<List<WinkelwagenUser>> GetAllWinkelwagenUsers()
        {
            // throw new NotImplementedException();
            Task<List<WinkelwagenUser>> shoppingCart = _shoppingcartRepository.GetAllWinkelwagenUsers();
            return await shoppingCart;
        }
        public async Task CreateService(ShoppingCartDTO shoppingcartDTO)
        {
           await _shoppingcartRepository.AddShoppingCarts(shoppingcartDTO);
        }
        public async void UpdateteService(ShoppingCartDTO shoppingCartDTO)
        {
            _shoppingcartRepository.UpdateShoppingcarts(shoppingCartDTO);
        }
        public async Task DeleteService(int id)
        {
            await _shoppingcartRepository.DeleteShoppingCarts(id);
        }
        public async void DeleteProductsService(ShoppingCartDTO shoppingCartDTO)
        {
            _shoppingcartRepository.DeleteProductFromShoppingcarts(shoppingCartDTO);
        }
    }
}