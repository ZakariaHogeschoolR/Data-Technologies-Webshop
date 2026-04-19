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
        var result = users.Select(u =>
            new AdminUserDto(u.Id, u.FirstName, u.LastName, u.Username, u.Email, u.Address, u.PostCode, u.Role));

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

    [HttpPost("users/{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(int id, [FromBody] AdminResetPasswordDto data)
    {
        await userService.ResetPasswordService(id, data.NewPassword);
        return Ok(new { message = "Password reset successful" });
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await userService.DeleteService(id);
        return Ok(new { message = "User deleted successfully" });
    }
}
