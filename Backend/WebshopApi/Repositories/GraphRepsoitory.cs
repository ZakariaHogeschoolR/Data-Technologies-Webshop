using ApplicationDbContext;

using DataTransferObject;

public class GraphRepository
{
    private readonly DatabaseConnectie _dbConnectie;
    private readonly Neo4jService _neo4j;

    public GraphRepository(DatabaseConnectie dbConnectie, Neo4jService neo4j)
    {
        _dbConnectie = dbConnectie;
        _neo4j = neo4j;
    }

    public async Task AddBought(BoughtDto dto)
    {
        await using var session = _neo4j.CreateSession();

        const string query = """
                                 MERGE (u:User {id: $userId})
                                 MERGE (p:Product {id: $productId})

                                 MERGE (u)-[r:BOUGHT]->(p)
                                 ON CREATE SET
                                     r.count = 1,
                                     r.firstBought = datetime(),
                                     r.lastBought = datetime()
                                 ON MATCH SET
                                     r.count = r.count + 1,
                                     r.lastBought = datetime()
                             """;

        await session.RunAsync(query, new
        {
            userId = dto.UserId,
            productId = dto.ProductId
        });
    }

    public async Task AddBulkBought(BulkBoughtDto dto)
    {
        if (dto.ProductIds == null || dto.ProductIds.Count == 0) return;

        await using var session = _neo4j.CreateSession();

        const string query = """
                             MERGE (u:User {id: $userId}) WITH u
                             UNWIND $productIds AS prodId
                             MERGE (p:Product {id: prodId})

                             MERGE (u)-[r:BOUGHT]->(p)
                             ON CREATE SET
                                r.count = 1,
                                r.firstBought = datetime(),
                                r.lastBought = datetime()
                             ON MATCH SET
                                r.count = r.count + 1,
                                r.lastBought = datetime()
                             """;

        await session.RunAsync(query, new
        {
            userId = dto.UserId,
            productIds = dto.ProductIds
        });
    }
}
