using Npgsql;
using models;
using ApplicationDbContext;
using DataTransferObject;

public class TeamRepository
{
    private readonly DatabaseConnectie _dbConnectie;

    public TeamRepository(DatabaseConnectie dbConnectie)
    {
        _dbConnectie = dbConnectie;
    }

    public async Task<List<Team>> GetAllTeams()
    {
        var teams = new List<Team>();
        using var conn = await _dbConnectie.GetConnection();
        var sql = "SELECT * FROM teams";

        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            teams.Add(new Team
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
            });
        }

        return teams;
    }

    public async Task<Team?> GetTeamById(int id)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = "SELECT * FROM teams WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Team
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                Name = reader.GetString(reader.GetOrdinal("name")),
            };
        }

        return null;
    }

    public async Task AddTeam(TeamDto team)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = @"INSERT INTO teams (name)
                    VALUES (@name)";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@name", team.Name);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateTeam(TeamDto team)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = @"UPDATE teams
                    SET name = @name
                    WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", team.Id);
        cmd.Parameters.AddWithValue("@name", team.Name);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteTeam(int id)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = "DELETE FROM teams WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}