using ApplicationDbContext;

using DataTransferObject;

using models;

using Npgsql;

using Xunit;

public class TeamIntegrationTest : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public TeamIntegrationTest(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    private TeamRepository CreateRepository()
    {
        var db = new DatabaseConnectie(_fixture.Postgres.GetConnectionString());
        return new TeamRepository(db);
    }

    private async Task<NpgsqlConnection> OpenConnectionAsync()
    {
        var connection = new NpgsqlConnection(_fixture.Postgres.GetConnectionString());
        await connection.OpenAsync();
        return connection;
    }

    private async Task CleanTeamsAsync(NpgsqlConnection connection)
    {
        await using var cmd = new NpgsqlCommand("DELETE FROM teams;", connection);
        await cmd.ExecuteNonQueryAsync();
    }

    private async Task<int> SeedTeamAsync(
        NpgsqlConnection connection,
        string name,
        string type = "sales")
    {
        var sql = @"
            INSERT INTO teams (name, type)
            VALUES (@name, @type)
            RETURNING id;";

        await using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@type", type);

        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }

    [Fact]
    public async Task GetAllTeams_ShouldReturnAllTeams()
    {
        await using var conn = await OpenConnectionAsync();

        await CleanTeamsAsync(conn);

        await SeedTeamAsync(conn, "Ajax");
        await SeedTeamAsync(conn, "PSV");

        var repository = CreateRepository();

        var result = await repository.GetAllTeams();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, t => t.Name == "Ajax");
        Assert.Contains(result, t => t.Name == "PSV");
    }

    [Fact]
    public async Task GetTeamById_ShouldReturnCorrectTeam()
    {
        await using var conn = await OpenConnectionAsync();

        await CleanTeamsAsync(conn);

        var teamId = await SeedTeamAsync(conn, "Feyenoord");

        var repository = CreateRepository();

        var result = await repository.GetTeamById(teamId);

        Assert.NotNull(result);
        Assert.Equal(teamId, result!.Id);
        Assert.Equal("Feyenoord", result.Name);
    }

    [Fact]
    public async Task AddTeam_ShouldInsertTeam()
    {
        await using var conn = await OpenConnectionAsync();

        await CleanTeamsAsync(conn);

        var repository = CreateRepository();

        var dto = new TeamDto
        {
            Name = "AZ"
        };

        await repository.AddTeam(dto);

        await using var verifyCmd =
            new NpgsqlCommand(
                "SELECT COUNT(*) FROM teams WHERE name = @name",
                conn);

        verifyCmd.Parameters.AddWithValue("@name", "AZ");

        var count = Convert.ToInt32(await verifyCmd.ExecuteScalarAsync());

        Assert.Equal(1, count);
    }

    [Fact]
    public async Task UpdateTeam_ShouldUpdateName()
    {
        await using var conn = await OpenConnectionAsync();

        await CleanTeamsAsync(conn);

        var teamId = await SeedTeamAsync(conn, "Old Team");

        var repository = CreateRepository();

        var dto = new TeamDto
        {
            Id = teamId,
            Name = "New Team"
        };

        await repository.UpdateTeam(dto);

        await using var verifyCmd =
            new NpgsqlCommand(
                "SELECT name FROM teams WHERE id = @id",
                conn);

        verifyCmd.Parameters.AddWithValue("@id", teamId);

        var updatedName = (string)(await verifyCmd.ExecuteScalarAsync())!;

        Assert.Equal("New Team", updatedName);
    }

    [Fact]
    public async Task DeleteTeam_ShouldRemoveTeam()
    {
        await using var conn = await OpenConnectionAsync();

        await CleanTeamsAsync(conn);

        var teamId = await SeedTeamAsync(conn, "Delete Me");

        var repository = CreateRepository();

        await repository.DeleteTeam(teamId);

        await using var verifyCmd =
            new NpgsqlCommand(
                "SELECT COUNT(*) FROM teams WHERE id = @id",
                conn);

        verifyCmd.Parameters.AddWithValue("@id", teamId);

        var count = Convert.ToInt32(await verifyCmd.ExecuteScalarAsync());

        Assert.Equal(0, count);
    }
}
