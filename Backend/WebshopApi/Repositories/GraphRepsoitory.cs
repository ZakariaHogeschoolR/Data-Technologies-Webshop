using ApplicationDbContext;

using DataTransferObject;

using models;

using Npgsql;

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

        int userId = dto.UserId;

        using var conn = await _dbConnectie.GetConnection();

        var sql = "SELECT user_id FROM winkelwagen_users WHERE id = @userId";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@userId", dto.UserId);

        var result = await cmd.ExecuteScalarAsync();
        int idUser = 0;
        if (result != null)
        {
            idUser = Convert.ToInt32(result);
        }

        await using var session = _neo4j.CreateSession();

        var query = @"
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
        ";

        await session.RunAsync(query, new
        {
            userId = idUser,
            productId = dto.ProductId
        });
    }

    public async Task AddBulkBought(BulkBoughtDto dto)
    {
        if (dto.ProductIds == null || !dto.ProductIds.Any()) return;

        await using var conn = await _dbConnectie.GetConnection();
        const string sql = "SELECT user_id FROM winkelwagen_users WHERE id = @userId";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@userId", dto.UserId);

        var result = await cmd.ExecuteScalarAsync();
        var idUser = 0;
        if (result != null) idUser = Convert.ToInt32(result);

        if (idUser == 0) return;

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
            userId = idUser,
            productIds = dto.ProductIds
        });
    }
}
