using Microsoft.AspNetCore.Mvc;
using Service;

namespace WebshopApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController(UserService userService, ProductService productService) : ControllerBase
{
    private bool IsAdmin()
    {
        var role = HttpContext.Session.GetString("UserRole");
        return role == "admin";
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        if (!IsAdmin()) return Forbid();
        var users = await userService.GetAllService();
        return Ok(users);
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetAllProducts()
    {
        if (!IsAdmin()) return Forbid();
        var products = await productService.GetAllService();
        return Ok(products);
    }
}