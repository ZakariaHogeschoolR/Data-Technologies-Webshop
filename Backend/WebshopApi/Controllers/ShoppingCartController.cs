using System.Security.Claims;

using DataTransferObject;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using models;

using Service;

namespace WebshopApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShoppingCartController(ShoppingCartService shoppingCartService) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<ActionResult<List<ShoppingCarts>>> GetAllShoppingCarts()
    {
        var shoppingcarts = await shoppingCartService.GetAllShoppingCarts();
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
        var shoppingcart = await shoppingCartService.GetShoppingCartById(userId);
        return Ok(shoppingcart);
    }

    [Authorize]
    [HttpGet("history")]
    public async Task<ActionResult> GetOrderHistory()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id)) return Unauthorized();

        var orders = await shoppingCartService.GetOrderHistoryService(int.Parse(id));
        return Ok(orders);
    }

    [HttpGet("users")]
    public async Task<ActionResult> GetAllWinkelwagenUsers()
    {
        var result = await shoppingCartService.GetAllWinkelwagenUsers();
        return Ok(result);
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult<ShoppingCarts>> CreateShoppingCarts([FromBody] ShoppingCartDTO shoppingCartDto)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // return Ok(userIdString);
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();
        var userId = int.Parse(userIdString);
        shoppingCartDto.UserId = userId;
        // return Ok(userId);
        var result = await shoppingCartService.CreateService(shoppingCartDto);
        return CreatedAtAction(nameof(GetAllShoppingCartsById), new { id = userId }, result);
    }

    [HttpPut("update")]
    public async Task<ActionResult<ShoppingCarts>> UpdateShoppingCarts([FromBody] ShoppingCartDTO shoppingCartDto)
    {
        shoppingCartService.UpdateteService(shoppingCartDto);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("delete")]
    public async Task<ActionResult> DeleteShoppingcart([FromBody] ShoppingCartDTO shoppingCartDTO)
    {
        shoppingCartService.DeleteProductsService(shoppingCartDTO);
        return NoContent();
    }
}
