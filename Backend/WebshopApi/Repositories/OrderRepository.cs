using ApplicationDbContext;

using DataTransferObject;

using models;

using Npgsql;

public class OrderRepository
{
    private readonly DatabaseConnectie _dbConnectie;

    public OrderRepository(DatabaseConnectie dbConnectie)
    {
        _dbConnectie = dbConnectie;
    }

    public async Task<List<Orders>> GetAllOrders()
    {
        var orders = new List<Orders>();
        using var conn = await _dbConnectie.GetConnection();
        var sql = "SELECT * FROM orders;";

        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            orders.Add(new Orders
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                WinkelwagenUsersId = reader.GetInt32(reader.GetOrdinal("winkelwagen_users_id")),
                Total = reader.GetInt32(reader.GetOrdinal("total")),
                PaymentStatus = reader.GetBoolean(reader.GetOrdinal("payment_status")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            });
        }

        return orders;
    }

    public async Task<Orders?> GetOrderByWinkelwagenUsersId(int id)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = "SELECT * FROM orders WHERE winkelwagen_users_id = @winkelwagenUsersId ";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@winkelwagenUsersId", id);

        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Orders
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                WinkelwagenUsersId = reader.GetInt32(reader.GetOrdinal("winkelwagen_users_id")),
                Total = reader.GetInt32(reader.GetOrdinal("total")),
                PaymentStatus = reader.GetBoolean(reader.GetOrdinal("payment_status")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            };
        }

        return null;
    }

    public async Task<Orders?> GetOrderById(int id)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = "SELECT * FROM orders WHERE id = @id ";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Orders
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                WinkelwagenUsersId = reader.GetInt32(reader.GetOrdinal("winkelwagen_users_id")),
                Total = reader.GetInt32(reader.GetOrdinal("total")),
                PaymentStatus = reader.GetBoolean(reader.GetOrdinal("payment_status")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            };
        }

        return null;
    }


    public async Task AddOrder(OrderDto order)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = @"INSERT INTO orders (winkelwagen_users_id, total, payment_status, created_at)
                    VALUES (@winkelwagenUsersId, @total, @paymentStatus, @createdAt)";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@winkelwagenUsersId", order.WinkelwagenUsersId);
        cmd.Parameters.AddWithValue("@total", order.Total);
        cmd.Parameters.AddWithValue("@paymentStatus", order.PaymentStatus);
        cmd.Parameters.AddWithValue("createdAt", order.CreatedAt);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateOrder(OrderDto order)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = @"UPDATE orders
                    SET winkelwage_users_id = @winkelwagenUsersId,
                        total = @total,
                        description = @description,
                        payment_status = @paymentStatus
                    WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", order.Id);
        cmd.Parameters.AddWithValue("@winkelwagenUsersId", order.WinkelwagenUsersId);
        cmd.Parameters.AddWithValue("@total", order.Total);
        cmd.Parameters.AddWithValue("@paymentStatus", order.PaymentStatus);
        cmd.Parameters.AddWithValue("createdAt", order.CreatedAt);

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteOrderWinkelwagen(int id)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = "DELETE FROM orders WHERE winkelwagen_users_id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteOrder(int id)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = "DELETE FROM orders WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}
