using DataTransferObject;
using models;

public interface ITeam
{
    Task<List<Team>> GetAllTeams();

    Task<Team?> GetTeamById(int id);

    Task AddTeam(TeamDto team);

    Task UpdateTeam(TeamDto team);

    Task DeleteTeam(int id);
}