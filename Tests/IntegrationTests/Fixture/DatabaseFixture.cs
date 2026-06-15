using Npgsql;

using Testcontainers.Neo4j;
using Testcontainers.PostgreSql;

public class DatabaseFixture : IAsyncLifetime
{
    public PostgreSqlContainer Postgres { get; private set; } = null!;
    public Neo4jContainer Neo4j { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        Postgres = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        Neo4j = new Neo4jBuilder().Build();

        await Postgres.StartAsync();

        await RunSchemaScript();

        await Neo4j.StartAsync();
    }
    public async Task DisposeAsync()
    {
        if (Postgres is not null)
        {
            await Postgres.DisposeAsync();
        }
        if (Neo4j is not null)
        {
            await Neo4j.DisposeAsync();
        }
    }
    public async Task RunSchemaScript()
    {
        var connectionString = Postgres.GetConnectionString();

        await using var connection = new Npgsql.NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var schemaPath = Path.Combine(AppContext.BaseDirectory, "Sql", "schema.sql");

        var sql = await File.ReadAllTextAsync(schemaPath);

        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }
}
