using Microsoft.AspNetCore.Mvc;
using models;
using Service;
using DataTransferObject;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class ShoppingCartController : ControllerBase
{
    private readonly ShoppingCartService _shoppingcartservice;
    public ShoppingCartController(ShoppingCartService shoppingcartService)
    {
        _shoppingcartservice = shoppingcartService;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet()]
    public async Task<ActionResult<List<ShoppingCarts>>> GetAllShoppingCarts()
    {
        var shoppingcarts = await _shoppingcartservice.GetAllShoppingCarts();
        return Ok(shoppingcarts);
    }
    
    [Authorize]
    [HttpGet("mine")]
    public async Task<ActionResult<ShoppingCarts>> GetAllShoppingCartsById()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // return Ok(userIdString);
        if(string.IsNullOrEmpty(userIdString)) return Unauthorized();
        var userId = int.Parse(userIdString);
        // return Ok(userId);
        var shoppingcart = await _shoppingcartservice.GetShoppingCartById(userId);
        return Ok(shoppingcart);
    }

    [HttpGet("users")]
    public async Task<ActionResult> GetAllWinkelwagenUsers()
    {
        var result = await _shoppingcartservice.GetAllWinkelwagenUsers();
        return Ok(result);
    }
    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult<ShoppingCarts>> CreateShoppingCarts([FromBody] ShoppingCartDTO shoppingCartDTO)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // return Ok(userIdString);
        if(string.IsNullOrEmpty(userIdString)) return Unauthorized();
        var userId = int.Parse(userIdString);
        // return Ok(userId);
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