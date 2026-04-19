using System.Security.Claims;

using DataTransferObject;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using models;

using Service;

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
    [HttpGet]
    public async Task<ActionResult<List<ShoppingCarts>>> GetAllShoppingCarts()
    {
        var shoppingcarts = await _shoppingcartservice.GetAllShoppingCarts();
        return Ok(shoppingcarts);
    }

    [Authorize]
    [HttpGet("mine")]
    public async Task<ActionResult<List<ShoppingCarts>>> GetAllShoppingCartsById()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // return Ok(userIdString);
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
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
    public async Task<ActionResult<ShoppingCarts>> CreateShoppingCarts([FromBody] ShoppingCartDto shoppingCartDTO)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // return Ok(userIdString);
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
        var userId = int.Parse(userIdString);
        shoppingCartDTO = shoppingCartDTO with { UserId = userId };
        // return Ok(userId);
        var result = await _shoppingcartservice.CreateService(shoppingCartDTO);
        return CreatedAtAction(nameof(GetAllShoppingCartsById), new { id = userId }, result);
    }

    [HttpPut("update")]
    public async Task<ActionResult<ShoppingCarts>> UpdateShoppingCarts([FromBody] ShoppingCartDto shoppingCartDTO)
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
