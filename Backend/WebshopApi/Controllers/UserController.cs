using Microsoft.AspNetCore.Mvc;
using models;
using Service;

[ApiController]
[Route("api/[controller]")]
public class UserController: ControllerBase
{
    private readonly UserService _userService;
    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet()]
    public async Task<ActionResult<List<Users>>> GetAllUsers()
    {
        var users = _userService.GetAllService();
        return Ok(users);
    }

    [HttpGet("/id")]
    public async Task<ActionResult<Users>> GetAllUsersById(int id)
    {
        var user = _userService.GetByIdService(id);
        return Ok(user);
    }

    [HttpPost("/create")]
    public async Task<ActionResult> CreateUser(Users user)
    {
        _userService.CreateService(user);
        return Ok();
    }

    [HttpPost("/Update")]
    public async Task<ActionResult> UpdateUser(Users user)
    {
        _userService.UpdateService(user);
        return Ok();
    }

    [HttpDelete("/Delete")]
    public async Task<ActionResult> DeleteUser(Users user)
    {
        _userService.DeleteService(user.Id);
        return Ok();
    }
}