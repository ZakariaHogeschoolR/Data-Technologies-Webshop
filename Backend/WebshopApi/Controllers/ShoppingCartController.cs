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

    [HttpGet("users")]
    public async Task<ActionResult> GetAllWinkelwagenUsers()
    {
        var result = await _shoppingcartservice.GetAllWinkelwagenUsers();
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<ActionResult<ShoppingCarts>> CreateShoppingCarts([FromBody] ShoppingCartDTO shoppingCartDTO)
    {
        await _shoppingcartservice.CreateService(shoppingCartDTO);
        return Created();
    }
    [HttpPut("update")]
    public async Task<ActionResult<ShoppingCarts>> UpdateShoppingCarts([FromBody] ShoppingCartDTO shoppingCartDTO)
    {
        _shoppingcartservice.UpdateteService(shoppingCartDTO);
        return NoContent();
    }
    [HttpDelete("delete/{id:int}")]
    public async Task<ActionResult> DeleteShoppingcart(int id)
    {
        _shoppingcartservice.DeleteService(id);
        return NoContent();
    }
}