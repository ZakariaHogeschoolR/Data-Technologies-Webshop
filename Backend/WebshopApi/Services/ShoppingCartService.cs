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
        var shoppingcarts = _shoppingcartRepository.GetAllShoppingCarts();
        return await shoppingcarts;
    }

    public async Task<List<ShoppingCarts>> GetShoppingCartById(int id) =>
        // throw new NotImplementedException();
        await _shoppingcartRepository.GetShoppingCartById(id);

    public async Task<List<WinkelwagenUser>> GetAllWinkelwagenUsers()
    {
        // throw new NotImplementedException();
        var shoppingCart = _shoppingcartRepository.GetAllWinkelwagenUsers();
        return await shoppingCart;
    }

    public async Task<ShoppingCarts> CreateService(ShoppingCartDto shoppingcartDTO) =>
        await _shoppingcartRepository.AddShoppingCarts(shoppingcartDTO);

    public async void UpdateteService(ShoppingCartDto shoppingCartDTO) =>
        _shoppingcartRepository.UpdateShoppingcarts(shoppingCartDTO);

    public async Task DeleteService(int id) => await _shoppingcartRepository.DeleteShoppingCarts(id);

    public async void DeleteProductsService(ShoppingCartDto shoppingCartDTO) =>
        _shoppingcartRepository.DeleteProductFromShoppingcarts(shoppingCartDTO);
}
