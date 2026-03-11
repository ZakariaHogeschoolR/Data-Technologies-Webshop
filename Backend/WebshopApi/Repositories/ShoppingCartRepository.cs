using ApplicationDbContext;
using models;

public class ShoppingCartRepository
{
    private readonly DatabaseConnectie _dbconnectie;
    public ShoppingCartRepository(DatabaseConnectie dbconnectie)
    {
        _dbconnectie = dbconnectie;
    }
    public async Task<List<ShoppingCarts?>> GetAllShoppingCarts(){}
    public async Task<ShoppingCarts?> GetShoppingCartById(){}
    public async Task<ShoppingCarts?> AddShoppingcarts(){}
    public async void AddProduct(Products product){}
    public async void UpdateShoppingcarts(){}
    public async void DeleteShoppingCarts(){}
    public async void DeleteProductFromShoppingcarts(){}
}