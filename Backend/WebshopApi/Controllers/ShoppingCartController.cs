using Microsoft.AspNetCore.Mvc;
using models;
using Service;
using DataTransferObject;

[ApiController]
[Route("api/[controller]")]
public class ShoppingCartController : ControllerBase
{
    private readonly ShoppingCartService _shoppingcartservice;
    public ShoppingCartController(ShoppingCartService shoppingcartService)
    {
        _shoppingcartservice = shoppingcartService;
    }

    [HttpGet()]
    public async Task<ActionResult<List<ShoppingCarts>>> GetAllShoppingCarts()
    {
        var shoppingcarts = await _shoppingcartservice.GetAllShoppingCarts();
        return Ok(shoppingcarts);
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ShoppingCarts>> GetAllShoppingCartsById(int id)
    {
        var shoppingcart = await _shoppingcartservice.GetShoppingCartById(id);
        return Ok(shoppingcart);
    }

    [HttpPost("create")]
    public async Task<ActionResult<ShoppingCarts>> CreateShoppingCarts([FromBody] ShoppingCartDTO shoppingCartDTO)
    {
        _shoppingcartservice.CreateService(shoppingCartDTO);
        return Ok();
    }
    [HttpPut("update")]
    public async Task<ActionResult<ShoppingCarts>> UpdateShoppingCarts(ProductDto productDto, int quantity)
    {
        _shoppingcartservice.UpdateteService(productDto, quantity);
        return Ok();
    }
    [HttpDelete("delete/{id:int}")]
    public async Task<ActionResult> DeleteShoppingcart(int id)
    {
        _shoppingcartservice.DeleteService(id);
        return Ok();
    }
}