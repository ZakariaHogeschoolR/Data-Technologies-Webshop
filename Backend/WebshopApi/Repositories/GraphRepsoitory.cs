using ApplicationDbContext;

using DataTransferObject;

using Neo4j.Driver;

public class GraphRepository : IGraph
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

    public async Task<List<TrendingProductDto>> TrendingProducts()
    {
        await using var session = _neo4j.CreateSession();

        // MATCH dwingt af: User -> BOUGHT -> Product -> BELONGS_TO -> Team
        const string query = """
            MATCH (:User)-[b:BOUGHT]->(p:Product)-[:BELONGS_TO]->(t:Team)
            RETURN
                p.id AS id,
                p.name AS name,
                p.price AS price,
                p.productImage AS imageUrl,
                t.id AS teamId,
                sum(b.count) AS purchases
            ORDER BY purchases DESC
            LIMIT 3
            """;

        var cursor = await session.RunAsync(query);
        var products = new List<TrendingProductDto>();

        while (await cursor.FetchAsync())
        {
            var record = cursor.Current;

            products.Add(new TrendingProductDto
            {
                // Convert is veiliger dan .As<T>() als Neo4j strings gebruikt voor ID's
                Id = Convert.ToInt32(record["id"]),
                Name = record["name"].ToString(),
                Price = Convert.ToDecimal(record["price"]),
                ImageUrl = record["imageUrl"]?.ToString() ?? "",
                TeamId = Convert.ToInt32(record["teamId"]), // Hier halen we t.id nu gegarandeerd binnen
                Purchases = Convert.ToInt64(record["purchases"])
            });
        }

        return products;
    }
}
