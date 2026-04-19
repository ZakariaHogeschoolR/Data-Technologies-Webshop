using DataTransferObject;

using Microsoft.AspNetCore.Mvc;

using models;

using Service;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet()]
    public async Task<ActionResult<List<Users>>> GetAllUsers()
    {
        var users = await _userService.GetAllService();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Users>> GetAllUsersById(int id)
    {
        var user = await _userService.GetByIdService(id);
        return Ok(user);
    }

    [HttpPost("create")]
    public async Task<ActionResult> CreateUser(UserDto user)
    {
        _userService.CreateService(user);
        return Ok();
    }

    [HttpPost("Update")]
    public async Task<ActionResult> UpdateUser(UserDto user)
    {
        _userService.UpdateService(user);
        return Ok();
    }

    [HttpDelete("Delete/{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        _userService.DeleteService(id);
        return Ok();
    }
}
