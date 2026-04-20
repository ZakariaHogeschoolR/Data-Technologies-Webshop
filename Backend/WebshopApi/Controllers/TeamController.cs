using DataTransferObject;

using Microsoft.AspNetCore.Mvc;

using models;

using Service;

namespace WebshopApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamController : ControllerBase
{
    private readonly TeamService _teamService;

    public TeamController(TeamService teamService)
    {
        _teamService = teamService;
    }

    [HttpGet]
    public async Task<ActionResult<List<Team>>> GetAllTeams()
    {
        var teams = await _teamService.GetAllService();
        return Ok(teams);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Team>> GetTeamById(int id)
    {
        var team = await _teamService.GetByIdService(id);

        if (team == null) return NotFound($"Team with id {id} was not found.");

        return Ok(team);
    }

    [HttpPost("create")]
    public async Task<ActionResult> CreateTeam([FromBody] TeamDto team)
    {
        if (team == null) return BadRequest("Team data is required.");

        await _teamService.CreateService(team);
        return Ok("Team created successfully.");
    }

    [HttpPut("update")]
    public async Task<ActionResult> UpdateTeam([FromBody] TeamDto team)
    {
        if (team == null || team.Id <= 0) return BadRequest("A valid team id is required.");

        var existingTeam = await _teamService.GetByIdService(team.Id);
        if (existingTeam == null) return NotFound($"Team with id {team.Id} was not found.");

        await _teamService.UpdateService(team);
        return Ok("Team updated successfully.");
    }

    [HttpDelete("delete/{id}")]
    public async Task<ActionResult> DeleteTeam(int id)
    {
        var existingTeam = await _teamService.GetByIdService(id);
        if (existingTeam == null) return NotFound($"Team with id {id} was not found.");

        await _teamService.DeleteService(id);
        return Ok("Team deleted successfully.");
    }
}
