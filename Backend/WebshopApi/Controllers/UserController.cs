using System.Security.Claims;

using DataTransferObject;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using models;

using Service;

namespace WebshopApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(UserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Users>>> GetAllUsers()
    {
        var users = await userService.GetAllService();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Users>> GetAllUsersById(int id)
    {
        var user = await userService.GetByIdService(id);
        return Ok(user);
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<ProfileDto>> GetProfile()
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id)) return Unauthorized();

        var user = await userService.GetByIdService(int.Parse(id));

        return Ok(new ProfileDto(user.Id, user.FirstName, user.LastName, user.Username, user.Email, user.Address,
            user.PostCode));
    }

    [Authorize]
    [HttpPut("me/update")]
    public async Task<ActionResult> UpdateProfile(UpdateProfileDto updateProfileDto)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id)) return Unauthorized();

        var userId = int.Parse(id);
        var user = await userService.GetByIdService(userId);

        var updated = new UserDto(userId, updateProfileDto.FirstName, updateProfileDto.LastName,
            updateProfileDto.Username, updateProfileDto.Email, user.Password, updateProfileDto.Address,
            updateProfileDto.PostCode);

        await userService.UpdateService(updated);

        return Ok(new { message = "Profile updated successfully" });
    }

    [Authorize]
    [HttpPut("me/update/password")]
    public async Task<ActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(id)) return Unauthorized();

        var user = await userService.GetByIdService(int.Parse(id));

        if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.Password))
            return Unauthorized(new { message = "Current password is incorrect" });

        await userService.ResetPasswordService(int.Parse(id), changePasswordDto.NewPassword);
        return Ok(new { message = "Password changed successfully" });
    }

    [HttpPost("create")]
    public async Task<ActionResult> CreateUser(UserDto user)
    {
        await userService.CreateService(user);
        return Ok();
    }

    [HttpPost("Update")]
    public async Task<ActionResult> UpdateUser(UserDto user)
    {
        await userService.UpdateService(user);
        return Ok();
    }

    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        await userService.DeleteService(id);
        return Ok();
    }
}
