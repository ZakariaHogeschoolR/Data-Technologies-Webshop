
using System.Security.Claims;

using DataTransferObject;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using models;

using Service;

namespace WebshopApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WishlistController : ControllerBase
{
    private readonly WishlistService _wishlistservice;

    public WishlistController(WishlistService wishlistService)
    {
        _wishlistservice = wishlistService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Wishlists>>> GetAllWishlists()
    {
        var wishlists = await _wishlistservice.GetAllWishLists();
        return Ok(wishlists);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<List<Wishlists>>> GetWishlistById(int id)
    {
        var wishlist = await _wishlistservice.GetWishlistsById(id);
        return Ok(wishlist);
    }
    [Authorize]
    [HttpGet("mine")]
    public async Task<ActionResult<Wishlists>> GetMyWishlist()
    {
        var useridstring = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // return Ok(useridstring);
        if (string.IsNullOrEmpty(useridstring)) return Unauthorized();
        var userId = int.Parse(useridstring);
        var wishlists = await _wishlistservice.GetWishlistsByUserId(userId);
        return Ok(wishlists);
    }
    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult> CreateWishlist(WishlistDTO wishlistDTO)
    {
        var useridstring = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // return Ok(useridstring);
        if (string.IsNullOrEmpty(useridstring)) return Unauthorized();
        int userid = int.Parse(useridstring);
        wishlistDTO.UserId = userid;
        var result = await _wishlistservice.CreateService(wishlistDTO);
        return Ok(result);
    }

    [HttpPut("update")]
    public async Task<ActionResult> UpdateWishlist(WishlistDTO wishlistDTO)
    {
        await _wishlistservice.UpdateService(wishlistDTO);
        return Ok();
    }

    [Authorize]
    [HttpDelete("delete/{id:int}")]
    public async Task<ActionResult> DeleteWishlist([FromRoute] int id)
    {
        var useridstring = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // return Ok(useridstring);
        if (string.IsNullOrEmpty(useridstring)) return Unauthorized();
        int userid = int.Parse(useridstring);
        await _wishlistservice.DeleteService(id);
        return Ok();
    }
    [Authorize]
    [HttpDelete("delete/product")]
    public async Task<ActionResult> DeleteProductFromWishlist([FromBody]DeleteProductDto deleteProductDto)
    {
        var useridstring = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // return Ok(useridstring);
        if (string.IsNullOrEmpty(useridstring)) return Unauthorized();
        int userid = int.Parse(useridstring);
        await _wishlistservice.DeleteProduct(deleteProductDto.Id, deleteProductDto.WishlistName);
        return Ok();
    }
}
