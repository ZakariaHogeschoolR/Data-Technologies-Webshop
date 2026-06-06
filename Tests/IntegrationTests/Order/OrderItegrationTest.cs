using System.Data.Common;
using System.Data.SqlTypes;
using System.Reflection;
using System.Threading.Tasks;

using ApplicationDbContext;

using DataTransferObject;

using Microsoft.Extensions.Configuration;

using Npgsql;

using Xunit;

public class OrderIntegrationTest : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    // xUnit zorgt ervoor dat de DatabaseFixture hier automatisch wordt binnengestuurd
    public OrderIntegrationTest(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetAllOrder_ShouldSaveAndRetrieveCorrectData()
    {
        var connectionString = _fixture.Postgres.GetConnectionString();

        var testOrderWinkelWagenId = 1;
        var testOrderTotal = 100;
        var testOrderPaymentStatus = false;
        DateTime? testOrderCreatedAt = DateTime.Now;

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using (var cleanupCommand = new NpgsqlCommand("DELETE FROM orders;", connection))
        {
            await cleanupCommand.ExecuteNonQueryAsync();
        }
        await using var seed = new NpgsqlCommand(@"
            INSERT INTO winkelwagen_users (id)
            VALUES (1)
            ON CONFLICT DO NOTHING;
        ", connection);
        var insertQuery =
            @"INSERT INTO orders (winkelwagen_users_id, total, payment_status, created_at)
            VALUES (@winkelwagenUsersId, @total, @paymentStatus, @createdAt)
            RETURNING id;";

        await using var insertCommand = new NpgsqlCommand(insertQuery, connection);
        insertCommand.Parameters.AddWithValue("@winkelwagenUsersId", testOrderWinkelWagenId);
        insertCommand.Parameters.AddWithValue("@total", testOrderTotal);
        insertCommand.Parameters.AddWithValue("@paymentStatus", testOrderPaymentStatus);
        insertCommand.Parameters.AddWithValue("@createdAt", (object?)testOrderCreatedAt ?? DBNull.Value);

        var generatedId = (int)await insertCommand.ExecuteScalarAsync();

        var selectQuery =
            @"SELECT *
            FROM orders
            WHERE id = @id;";

        await using var selectCommand = new NpgsqlCommand(selectQuery, connection);
        selectCommand.Parameters.AddWithValue("@id", generatedId);

        await using var reader = await selectCommand.ExecuteReaderAsync();

        Assert.True(await reader.ReadAsync());

        var retrievedWinkelwagenId = reader.GetInt32(1);
        var retrievedTotal = reader.GetInt32(2);
        var retrievedPaymentStatus = reader.GetBoolean(3);

        Assert.Equal(testOrderWinkelWagenId, retrievedWinkelwagenId);
        Assert.Equal(testOrderTotal, retrievedTotal);
        Assert.Equal(testOrderPaymentStatus, retrievedPaymentStatus);
    }

    [Fact]
    public async Task GetCategoryById_ShouldSaveAndRetrieveCorrectData()
    {

        var connectionString = _fixture.Postgres.GetConnectionString();
        var connectionNeo4jString = _fixture.Neo4j.GetConnectionString();
        var db = new DatabaseConnectie(connectionString);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Neo4j:Uri"] = _fixture.Neo4j.GetConnectionString(),
                ["Neo4j:User"] = "neo4j",
                ["Neo4j:Password"] = "password"
            })
            .Build();

        var neo4j = new Neo4jService(config);
        var testOrderWinkelWagenId = 1;
        var testOrderTotal = 100;
        var testOrderPaymentStatus = false;
        DateTime? testOrderCreatedAt = DateTime.Now;

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using (var cleanupCommand = new NpgsqlCommand("DELETE FROM orders;", connection))
        {
            await cleanupCommand.ExecuteNonQueryAsync();
        }
        await using var seed = new NpgsqlCommand(@"
            INSERT INTO winkelwagen_users (id)
            VALUES (1)
            ON CONFLICT DO NOTHING;
        ", connection);

        await seed.ExecuteNonQueryAsync();
        var insertQuery =
            @"INSERT INTO orders (winkelwagen_users_id, total, payment_status, created_at)
            VALUES (@winkelwagenUsersId, @total, @paymentStatus, @createdAt)
            RETURNING id;";

        await using var insertCommand = new NpgsqlCommand(insertQuery, connection);
        insertCommand.Parameters.AddWithValue("@winkelwagenUsersId", testOrderWinkelWagenId);
        insertCommand.Parameters.AddWithValue("@total", testOrderTotal);
        insertCommand.Parameters.AddWithValue("@paymentStatus", testOrderPaymentStatus);
        insertCommand.Parameters.AddWithValue("@createdAt", (object?)testOrderCreatedAt ?? DBNull.Value);

        var generatedId = (int)await insertCommand.ExecuteScalarAsync();

        var selectQuery =
            @"SELECT *
            FROM orders
            WHERE id = @id;";

        await using var selectCommand = new NpgsqlCommand(selectQuery, connection);
        selectCommand.Parameters.AddWithValue("@id", generatedId);
        var repository = new OrderRepository(db);
        var Order = await repository.GetOrderById((int)generatedId);

        await using var reader = await selectCommand.ExecuteReaderAsync();

        Assert.True(await reader.ReadAsync());

        var retrievedWinkelwagenId = reader.GetInt32(1);
        var retrievedTotal = reader.GetInt32(2);
        var retrievedPaymentStatus = reader.GetBoolean(3);

        Assert.Equal(testOrderWinkelWagenId, Order.WinkelwagenUsersId);
        Assert.Equal(testOrderTotal, Order.Total);
        Assert.Equal(testOrderPaymentStatus, Order.PaymentStatus);

    }

    [Fact]
    public async Task InsertCategory_ShouldSaveAndRetrieveCorrectData()
    {
        var connectionString = _fixture.Postgres.GetConnectionString();
        var connectionNeo4jString = _fixture.Neo4j.GetConnectionString();
        var db = new DatabaseConnectie(connectionString);
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Neo4j:Uri"] = _fixture.Neo4j.GetConnectionString(),
                ["Neo4j:User"] = "neo4j",
                ["Neo4j:Password"] = "password"
            })
            .Build();

        var neo4j = new Neo4jService(config);
        var testOrderWinkelWagenId = 1;
        var testOrderTotal = 100;
        var testOrderPaymentStatus = false;
        DateTime? testOrderCreatedAt = DateTime.Now;

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using (var cleanupCommand = new NpgsqlCommand("DELETE FROM orders;", connection))
        {
            await cleanupCommand.ExecuteNonQueryAsync();
        }
        await using var seed = new NpgsqlCommand(@"
            INSERT INTO winkelwagen_users (id)
            VALUES (1)
            ON CONFLICT DO NOTHING;
        ", connection);

        await seed.ExecuteNonQueryAsync();
        var insertQuery =
            @"INSERT INTO orders (winkelwagen_users_id, total, payment_status, created_at)
            VALUES (@winkelwagenUsersId, @total, @paymentStatus, @createdAt)
            RETURNING id;";

        await using var insertCommand = new NpgsqlCommand(insertQuery, connection);
        insertCommand.Parameters.AddWithValue("@winkelwagenUsersId", testOrderWinkelWagenId);
        insertCommand.Parameters.AddWithValue("@total", testOrderTotal);
        insertCommand.Parameters.AddWithValue("@paymentStatus", testOrderPaymentStatus);
        insertCommand.Parameters.AddWithValue("@createdAt", (object?)testOrderCreatedAt ?? DBNull.Value);

        var generatedId = (int)await insertCommand.ExecuteScalarAsync();

        var selectQuery =
            @"SELECT *
            FROM orders
            WHERE id = @id;";

        await using var selectCommand = new NpgsqlCommand(selectQuery, connection);
        selectCommand.Parameters.AddWithValue("@id", generatedId);
        var repository = new OrderRepository(db);
        var Order = await repository.GetOrderById((int)generatedId);

        await using var reader = await selectCommand.ExecuteReaderAsync();

        Assert.True(await reader.ReadAsync());

        var retrievedWinkelwagenId = reader.GetInt32(1);
        var retrievedTotal = reader.GetInt32(2);
        var retrievedPaymentStatus = reader.GetBoolean(3);

        Assert.Equal(testOrderWinkelWagenId, Order.WinkelwagenUsersId);
        Assert.Equal(testOrderTotal, Order.Total);
        Assert.Equal(testOrderPaymentStatus, Order.PaymentStatus);
    }

    [Fact]
    public async Task UpdateOrder_ShouldUpdateCorrectly()
    {
        var connectionString = _fixture.Postgres.GetConnectionString();
        var db = new DatabaseConnectie(connectionString);

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using (var cleanup = new NpgsqlCommand("DELETE FROM orders;", connection))
            await cleanup.ExecuteNonQueryAsync();
        await using var seed = new NpgsqlCommand(@"
            INSERT INTO winkelwagen_users (id)
            VALUES (1)
            ON CONFLICT DO NOTHING;
        ", connection);
        await using var seed2 = new NpgsqlCommand(@"
            INSERT INTO winkelwagen_users (id)
            VALUES (2)
            ON CONFLICT DO NOTHING;
        ", connection);
        await seed.ExecuteNonQueryAsync();
        await seed2.ExecuteNonQueryAsync();
        var insertSql = @"
            INSERT INTO orders (winkelwagen_users_id, total, payment_status, created_at)
            VALUES (@w, @t, @p, @c)
            RETURNING id;";

        await using var insertCmd = new NpgsqlCommand(insertSql, connection);
        insertCmd.Parameters.AddWithValue("@w", 1);
        insertCmd.Parameters.AddWithValue("@t", 100);
        insertCmd.Parameters.AddWithValue("@p", false);
        insertCmd.Parameters.AddWithValue("@c", DateTime.UtcNow);

        var id = (int)await insertCmd.ExecuteScalarAsync();

        // ACT (update via repository)
        var repo = new OrderRepository(db);

        var dto = new OrderDto
        {
            Id = id,
            WinkelwagenUsersId = 2,
            Total = 250,
            PaymentStatus = true,
            CreatedAt = DateTime.UtcNow
        };

        await repo.UpdateOrder(dto);

        var selectSql = "SELECT winkelwagen_users_id, total, payment_status FROM orders WHERE id = @id";

        await using var selectCmd = new NpgsqlCommand(selectSql, connection);
        selectCmd.Parameters.AddWithValue("@id", id);

        await using var reader = await selectCmd.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());

        Assert.Equal(2, reader.GetInt32(0));
        Assert.Equal(250, reader.GetDecimal(1));
        Assert.True(reader.GetBoolean(2));
    }

    [Fact]
    public async Task DeleteCategory_ShouldSaveAndRetrieveCorrectData()
    {
        var connectionString = _fixture.Postgres.GetConnectionString();
        var db = new DatabaseConnectie(connectionString);

        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        await using (var cleanup = new NpgsqlCommand("DELETE FROM orders;", connection))
            await cleanup.ExecuteNonQueryAsync();

        var insertSql = @"
            INSERT INTO orders (winkelwagen_users_id, total, payment_status, created_at)
            VALUES (@w, @t, @p, @c)
            RETURNING id;";

        await using var insertCmd = new NpgsqlCommand(insertSql, connection);
        insertCmd.Parameters.AddWithValue("@w", 1);
        insertCmd.Parameters.AddWithValue("@t", 100);
        insertCmd.Parameters.AddWithValue("@p", false);
        insertCmd.Parameters.AddWithValue("@c", DateTime.UtcNow);

        var id = (int)await insertCmd.ExecuteScalarAsync();

        // ACT (update via repository)
        var repo = new OrderRepository(db);

        var dto = new OrderDto
        {
            Id = id,
            WinkelwagenUsersId = 2,
            Total = 250,
            PaymentStatus = true,
            CreatedAt = DateTime.UtcNow
        };

        await repo.DeleteOrder(dto.Id);

        var selectSql = "SELECT winkelwagen_users_id, total, payment_status FROM orders WHERE id = @id";

        await using var selectCmd = new NpgsqlCommand(selectSql, connection);
        selectCmd.Parameters.AddWithValue("@id", id);

        await using var reader = await selectCmd.ExecuteReaderAsync();
        Assert.False(await reader.ReadAsync());
        if (await reader.ReadAsync())
        {
            Assert.Equal(0, reader.GetInt32(1));
            Assert.Equal(0, reader.GetDecimal(2));
            Assert.False(reader.GetBoolean(3));
        }
    }
}
