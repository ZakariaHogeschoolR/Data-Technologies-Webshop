using DataTransferObject;

using models;

public interface IShoppingCartService
{
    Task<List<ShoppingCarts?>> GetAllShoppingCarts();
    Task<List<ShoppingCarts>> GetShoppingCartById(int id);
    Task<List<WinkelwagenUser>> GetAllWinkelwagenUsers();
    Task<ShoppingCarts> CreateService(ShoppingCartDTO shoppingcartDTO);
    void UpdateteService(ShoppingCartDTO shoppingCartDTO);
    Task DeleteService(int id);
    Task DeleteProductsService(ShoppingCartDTO shoppingCartDTO);

    Task<List<OrderHistoryDto>?> GetOrderHistoryService(int userId);

    Task<CheckoutResultDto> Checkout(int userId, string paymentMethod = "card");
}
