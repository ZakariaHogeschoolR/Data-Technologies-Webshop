using DataTransferObject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace WebshopApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class AdminController(UserService userService, ProductService productService) : ControllerBase
{
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await userService.GetAllService();
        var result = users.Select(u => new AdminUserDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Username = u.Username,
            Email = u.Email,
            Address = u.Address,
            PostCode = u.PostCode,
            Role = u.Role
        });
        return Ok(result);
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetAllProductsAdmin()
    {
        var products = await productService.GetAllServiceAdmin();
        return Ok(products);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var users = await userService.GetAllService();
        var products = await productService.GetAllServiceAdmin();

        return Ok(new
        {
            totalUsers = users.Count,
            totalProducts = products.Count
        });
    }
}