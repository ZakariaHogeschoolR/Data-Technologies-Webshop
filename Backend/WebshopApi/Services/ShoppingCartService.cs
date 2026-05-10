using DataTransferObject;

using models;

namespace Service;

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
        Task<List<ShoppingCarts?>> shoppingcarts = _shoppingcartRepository.GetAllShoppingCarts();
        return await shoppingcarts;
    }
    public async Task<List<ShoppingCarts>> GetShoppingCartById(int id)
    {
        // throw new NotImplementedException();
        return await _shoppingcartRepository.GetShoppingCartById(id);
    }
    public async Task<List<WinkelwagenUser>> GetAllWinkelwagenUsers()
    {
        // throw new NotImplementedException();
        Task<List<WinkelwagenUser>> shoppingCart = _shoppingcartRepository.GetAllWinkelwagenUsers();
        return await shoppingCart;
    }
    public async Task<ShoppingCarts> CreateService(ShoppingCartDTO shoppingcartDTO)
    {
        return await _shoppingcartRepository.AddShoppingCarts(shoppingcartDTO);
    }
    public async void UpdateteService(ShoppingCartDTO shoppingCartDTO)
    {
        _shoppingcartRepository.UpdateShoppingcarts(shoppingCartDTO);
    }
    public async Task DeleteService(int id)
    {
        await _shoppingcartRepository.DeleteShoppingCarts(id);
    }
    public async Task DeleteProductsService(ShoppingCartDTO shoppingCartDTO)
    {
        await _shoppingcartRepository.DeleteProductFromShoppingcarts(shoppingCartDTO);
    }

    public async Task<List<OrderHistoryDto>?> GetOrderHistoryService(int userId) => await _shoppingcartRepository.GetOrderHistoryByUserId(userId);
}
