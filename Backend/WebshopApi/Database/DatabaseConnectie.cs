using Npgsql;

namespace ApplicationDbContext;

public class DatabaseConnectie
{
    private readonly string _connectionString;
    public DatabaseConnectie(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<NpgsqlConnection> GetConnection()
    {
        var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        return conn;

    }

    public async Task TestConnectionAsync()
    {
        using var connection = await GetConnection();
    }
}
