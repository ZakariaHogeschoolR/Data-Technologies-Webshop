using Neo4j.Driver;
public interface INeo4jService
{

    IAsyncSession CreateSession();
}
