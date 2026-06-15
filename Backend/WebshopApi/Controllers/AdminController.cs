using System.Security.Claims;

using DataTransferObject;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Service;

namespace WebshopApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin,hoofdadmin")]
public class AdminController(UserService userService, ProductService productService, TeamService teamService, PasswordResetService passwordResetService) : ControllerBase
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
    public async Task<IActionResult> GetAllProductsAdmin([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var products = await productService.GetAllServiceAdminPaged(page, pageSize);
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

    [HttpGet("products/search")]
    public async Task<IActionResult> SearchProducts([FromQuery] string name)
    {
        var products = await productService.GetByNameService(name);
        return Ok(products);
    }

    [HttpGet("products/filter")]
    public async Task<IActionResult> FilterByCategories([FromQuery] List<int> categoryIds, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var products = await productService.GetProductsByCategoriesService(categoryIds, page, pageSize);
        return Ok(products);
    }

    [HttpGet("teams/search")]
    public async Task<IActionResult> SearchTeams([FromQuery] string name)
    {
        var teams = await teamService.GetAllService();
        var filtered = teams.Where(t => t.Name.ToLower().Contains(name.ToLower())).ToList();
        return Ok(filtered);
    }

    [HttpGet("stats/top-products")]
    public async Task<IActionResult> GetTopProducts()
    {
        var topProducts = await productService.GetTopProductsService();
        return Ok(topProducts);
    }

    [HttpPost("users/{id}/reset-password")]
    public async Task<IActionResult> ResetPassword(int id)
    {
        var user = await userService.GetByIdService(id);
        if (user == null) return NotFound();

        await passwordResetService.SendResetEmail(user.Email);
        return Ok(new { message = "Password reset email sent." });
    }

    [HttpPost("teams/create")]
    public async Task<IActionResult> CreateTeam([FromBody] TeamDto data)
    {
        await teamService.CreateService(data);
        return Ok(new { message = "Team created successfully" });
    }

    [HttpPost("products/create")]
    public async Task<IActionResult> CreateProduct([FromBody] ProductDto data)
    {
        var id = await productService.CreateServiceReturnId(data);
        return Ok(new { message = "Product created successfully", id });
    }

    [HttpPost("products/category")]
    public async Task<IActionResult> AddProductCategory([FromBody] ProductCategoryDto data)
    {
        await productService.AddProductCategoryService(data.ProductId, data.CategoryId);
        return Ok(new { message = "Category added successfully" });
    }

    [HttpPut("users/{id}/role")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] AdminUpdateRoleDto data)
    {
        var targetUser = await userService.GetByIdService(id);
        if (targetUser.Role == "hoofdadmin")
            return Forbid();

        if (data.Role == "hoofdadmin")
            return BadRequest(new { message = "Cannot assign hoofdadmin role." });

        await userService.UpdateRoleService(id, data.Role);
        return Ok(new { message = "Role updated successfully" });
    }

    [HttpPut("products/{id}/price")]
    public async Task<IActionResult> UpdateProductPrice(int id, [FromBody] UpdatePriceDto data)
    {
        await productService.UpdatePriceService(id, data.Price);
        return Ok(new { message = "Price updated successfully" });
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var targetUser = await userService.GetByIdService(id);

        if (targetUser.Role == "hoofdadmin")
            return Forbid();

        var currentRole = User.FindFirst(ClaimTypes.Role)?.Value;
        if (targetUser.Role == "admin" && currentRole != "hoofdadmin")
            return Forbid();

        await userService.DeleteService(id);
        return Ok(new { message = "User deleted successfully" });
    }

    [HttpDelete("products/{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        await productService.DeleteService(id);
        return Ok(new { message = "Product deleted successfully" });
    }
}
