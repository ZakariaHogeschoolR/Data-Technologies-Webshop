using Npgsql;
using Testcontainers.PostgreSql;

public class rbacDatabaseFixture : IAsyncLifetime
{
    public PostgreSqlContainer Postgres { get; private set; } = null!;

    public string WebshopAppConnectionString =>
        BuildConnectionString("webshop_app", "webshop_app_test_pw");

    public string WebshopAdminConnectionString =>
        BuildConnectionString("webshop_admin", "webshop_admin_test_pw");

    public string SuperuserConnectionString =>
        Postgres.GetConnectionString();

    public async Task InitializeAsync()
    {
        Postgres = new PostgreSqlBuilder()
            .WithDatabase("data_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithEnvironment("POSTGRES_HOST_AUTH_METHOD", "trust")
            .Build();

        await Postgres.StartAsync();

        await RunSqlFileAsync("schema.sql");
        await RunSqlFileAsync("rbac_setup.sql");
    }

    public async Task DisposeAsync()
    {
        if (Postgres is not null)
            await Postgres.DisposeAsync();
    }

    private async Task RunSqlFileAsync(string fileName)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(Postgres.GetConnectionString());
        dataSourceBuilder.ConnectionStringBuilder.MaxAutoPrepare = 0;
        await using var dataSource = dataSourceBuilder.Build();
        await using var connection = await dataSource.OpenConnectionAsync();

        var path = Path.Combine(AppContext.BaseDirectory, "Sql", fileName);
        var sql = await File.ReadAllTextAsync(path);

        await using var cmd = new NpgsqlCommand(sql, connection);
        await cmd.ExecuteNonQueryAsync();
    }

    private string BuildConnectionString(string username, string password)
    {
        var builder = new NpgsqlConnectionStringBuilder(Postgres.GetConnectionString())
        {
            Username = username,
            Password = null
        };
        return builder.ConnectionString;
    }
}
