using DataTransferObject;
using models;

public interface IShoppingCart
{
    Task<List<ShoppingCarts?>> GetAllShoppingCarts();

    Task<List<WinkelwagenUser>> GetAllWinkelwagenUsers();
    
    Task<List<ShoppingCarts?>> GetShoppingCartById(int id);
    

    Task<ShoppingCarts> AddShoppingCarts(ShoppingCartDTO shoppingcarts);
    

    //in between table
    //void ChangeQuantity(Products product, int quantity);

    void UpdateShoppingcarts(ShoppingCartDTO shoppingCartDTO);

    Task DeleteShoppingCarts(int id);

    Task DeleteProductFromShoppingcarts(ShoppingCartDTO shoppingCartDTO);

    Task<List<OrderHistoryDto>?> GetOrderHistoryByUserId(int userId);
    

    Task<CheckoutResultDto> Checkout(int userId, string paymentMethod = "card");
}