using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly DatabaseFixture _fixture;

    public TestWebApplicationFactory(DatabaseFixture fixture)
    {
        _fixture = fixture;

        Environment.SetEnvironmentVariable("Jwt__Key", "test-secret-key-that-is-long-enough-32chars!");
        Environment.SetEnvironmentVariable("Jwt__Issuer", "test");
        Environment.SetEnvironmentVariable("Jwt__Audience", "test");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            var settings = new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = _fixture.Postgres.GetConnectionString(),
                ["Jwt:Key"] = "test-secret-key-that-is-long-enough-32chars!",
                ["Jwt:Issuer"] = "test",
                ["Jwt:Audience"] = "test"
            };
            config.AddInMemoryCollection(settings);
        });
    }
}
