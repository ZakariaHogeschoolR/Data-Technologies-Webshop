using DataTransferObject;

using Microsoft.AspNetCore.Mvc;

using Service;

namespace WebshopApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(UserService service, TokenService tokenService, PasswordResetService passwordResetService) : ControllerBase
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
        return Ok(new { message = "Logged in", token, username = user.Username, role = user.Role });
    }

    [HttpPost("logout")]
    public IActionResult Logout() => Ok(new { message = "Logged out" });

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto data)
    {
        await passwordResetService.SendResetEmail(data.Email);
        return Ok(new { message = "If this email exists, a reset link has been sent." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPassword data)
    {
        var success = await passwordResetService.ResetPassword(data.Token, data.Password);
        if (!success) return BadRequest(new { message = "Invalid or expired token." });
        return Ok(new { message = "Password reset successful." });
    }
}
