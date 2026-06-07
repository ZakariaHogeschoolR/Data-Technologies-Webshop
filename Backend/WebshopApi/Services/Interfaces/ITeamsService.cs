using DataTransferObject;

using models;

public interface ITeamService
{

    Task<List<Team>> GetAllService();

    Task<Team?> GetByIdService(int id);

    Task CreateService(TeamDto team);

    Task UpdateService(TeamDto team);

    Task DeleteService(int id);
}
