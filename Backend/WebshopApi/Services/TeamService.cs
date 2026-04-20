using DataTransferObject;

using models;

namespace Service;

public class TeamService
{
    private readonly TeamRepository _teamRepository;

    public TeamService(TeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    public async Task<List<Team>> GetAllService()
    {
        return await _teamRepository.GetAllTeams();
    }

    public async Task<Team?> GetByIdService(int id)
    {
        return await _teamRepository.GetTeamById(id);
    }

    public async Task CreateService(TeamDto team)
    {
        await _teamRepository.AddTeam(team);
    }

    public async Task UpdateService(TeamDto team)
    {
        await _teamRepository.UpdateTeam(team);
    }

    public async Task DeleteService(int id)
    {
        await _teamRepository.DeleteTeam(id);
    }
}
