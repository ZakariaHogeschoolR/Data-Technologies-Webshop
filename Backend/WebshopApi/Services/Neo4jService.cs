using Neo4j.Driver;

public class Neo4jService: INeo4jService
{
    private readonly IDriver _driver;

    public Neo4jService(IConfiguration config)
    {
        _driver = GraphDatabase.Driver(
            config["Neo4j:Uri"],
            AuthTokens.Basic(
                config["Neo4j:User"],
                config["Neo4j:Password"]
            )
        );
    }

    public IAsyncSession CreateSession()
        => _driver.AsyncSession();
}
