using ApplicationDbContext;

using DataTransferObject;

using models;

using Neo4j.Driver;

using Npgsql;

public class UserGraphRepository : IUserGraph
{
    private readonly Neo4jService _neo4j;

    public UserGraphRepository(Neo4jService neo4j)
    {
        _neo4j = neo4j;
    }

    public async Task FollowUser(string userId, string targetUserId)
    {
        await using var session = _neo4j.CreateSession();

        await session.RunAsync(@"
            MERGE (a:User {id: $userId})
            MERGE (b:User {id: $targetUserId})
            MERGE (a)-[:FOLLOWS]->(b)
        ", new { userId, targetUserId });
    }

    public async Task<List<Products>> GetRecommendation(int userId)
    {
        await using var session = _neo4j.CreateSession();

        var result = await session.RunAsync(@"
            MATCH (u:User {id: $userId})-[:BOUGHT]->(p:Product)
            MATCH (p)<-[:BOUGHT]-(other:User)
            MATCH (other)-[:BOUGHT]->(rec:Product)
            WHERE NOT (u)-[:BOUGHT]->(rec)
            AND other <> u
            RETURN rec, count(*) AS score
            ORDER BY score DESC
            LIMIT 10
        ", new { userId });

        var products = new List<Products>();

        while (await result.FetchAsync())
        {
            var node = (INode)result.Current["rec"];

            var product = new Products
            {
                Id = node.Properties.ContainsKey("id") ? Convert.ToInt32(node.Properties["id"]) : 0,
                Name = node.Properties.ContainsKey("name") ? node.Properties["name"].ToString() : null,
                ProductImage = node.Properties.ContainsKey("productImage") ? node.Properties["productImage"].ToString() : null,
                Price = node.Properties.ContainsKey("price")
                    ? Convert.ToDecimal(node.Properties["price"])
                    : 0
            };

            products.Add(product);
        }

        return products;
    }
}
