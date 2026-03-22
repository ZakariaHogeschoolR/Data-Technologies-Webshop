using DataTransferObject;

using Microsoft.AspNetCore.Mvc;

using models;

using Service;
[ApiController]
[Route("api/[controller]")]
public class WishlistController : ControllerBase
{
    private readonly WishlistService _wishlistservice;
    public WishlistController(WishlistService wishlistService)
    {
        _wishlistservice = wishlistService;
    }

    [HttpGet()]
    public async Task<ActionResult<List<Wishlists>>> GetAllWishlists()
    {
        var wishlists = _wishlistservice.GetAllWishLists();
        return Ok(wishlists);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Wishlists>> GetWishlistById(int id)
    {
        var wishlist = _wishlistservice.GetWishlistsById(id);
        return Ok(wishlist);
    }

    [HttpPost("create")]
    public async Task<ActionResult> CreateWishlist(WishlistDTO wishlistDTO)
    {
        _wishlistservice.CreateService(wishlistDTO);
        return Ok();
    }

    [HttpPut("update")]
    public async Task<ActionResult> UpdateWishlist(WishlistDTO wishlistDTO)
    {
        _wishlistservice.UpdateService(wishlistDTO);
        return Ok();
    }

    [HttpDelete("delete")]
    public async Task<ActionResult> DeleteWishlist(int id)
    {
        _wishlistservice.DeleteService(id);
        return Ok();
    }
}
