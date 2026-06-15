public class ApiStartupTests : IClassFixture<DatabaseFixture>
{
    private readonly HttpClient _client;

    public ApiStartupTests(DatabaseFixture fixture)
    {
        var factory = new TestWebApplicationFactory(fixture);

        _client = factory.CreateClient();
    }
    [Fact]
    public async Task ApiStartup()
    {
        var response = await _client.GetAsync("/");

        Assert.NotNull(response);
    }
}
