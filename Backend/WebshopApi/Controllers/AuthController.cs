using DataTransferObject;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace WebshopApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(UserService service, TokenService tokenService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(Register data)
    {
        await service.RegisterService(data);
        return Ok(new { message = "Registration Successful" });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(Login data)
    {
        var user = await service.LoginService(data);
        if (user == null) return Unauthorized(new { message = "Invalid email or password" });

        var token = tokenService.GenerateToken(user);
        return Ok(new { message = "Logged in", token });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logged out" });
    }
}