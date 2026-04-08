using DataTransferObject;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace WebshopApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController(UserService userService, ProductService productService) : ControllerBase
{
    private bool IsAdmin()
    {
        return User.IsInRole("Admin");
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        if (!IsAdmin()) return Forbid();
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
    public async Task<IActionResult> GetAllProducts()
    {
        if (!IsAdmin()) return Forbid();
        var products = await productService.GetAllService();
        return Ok(products);
    }
    
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        if (!IsAdmin()) return Forbid();
        
        var users = await userService.GetAllService();
        var products = await productService.GetAllService();
        
        return Ok(new
        {
            totalUsers = users.Count,
            totalProducts = products.Count
        });
    }
}