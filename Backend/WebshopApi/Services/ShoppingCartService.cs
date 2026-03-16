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
        public async Task<ShoppingCarts?> GetShoppingCartById(int id)
        {
            // throw new NotImplementedException();
            Task<ShoppingCarts?> shoppingCart = _shoppingcartRepository.GetShoppingCartById(id);
            return await shoppingCart;
        }
        public async void CreateService(ShoppingCartDTO shoppingcartDTO)
        {
           _shoppingcartRepository.AddShoppingCarts(shoppingcartDTO);
        }
        public async void UpdateteService(ProductDto productDto, int quantity)
        {
            _shoppingcartRepository.UpdateShoppingcarts(productDto.Id, quantity);
        }
        public async void DeleteService(int id)
        {
            _shoppingcartRepository.DeleteShoppingCarts(id);
        }
        public async void DeleteProductsService(int userid, int id)
        {
            _shoppingcartRepository.DeleteProductFromShoppingcarts(userid, id);
        }
    }
}