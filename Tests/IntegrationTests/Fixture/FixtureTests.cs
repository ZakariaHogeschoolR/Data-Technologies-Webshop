public class ContainerTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public ContainerTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void ContainersStarted()
    {
        Assert.NotNull(_fixture.Postgres);
        Assert.NotNull(_fixture.Neo4j);
    }
}
