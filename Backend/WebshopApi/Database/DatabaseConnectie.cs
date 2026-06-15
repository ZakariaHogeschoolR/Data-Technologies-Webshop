using Npgsql;

namespace ApplicationDbContext;

public class DatabaseConnectie(string connectionString)
{
    public async Task<NpgsqlConnection> GetConnection()
    {
        var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        return conn;
    }

    public async Task TestConnectionAsync()
    {
        await using var connection = await GetConnection();
    }
}
