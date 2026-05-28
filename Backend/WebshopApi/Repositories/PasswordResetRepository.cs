using ApplicationDbContext;

using Npgsql;

public class PasswordResetRepository
{
    private readonly DatabaseConnectie _dbConnectie;

    public PasswordResetRepository(DatabaseConnectie dbConnectie)
    {
        _dbConnectie = dbConnectie;
    }

    public async Task SaveToken(int userId, string token, DateTime expiresAt)
    {
        using var conn = await _dbConnectie.GetConnection();
        var sql = "INSERT INTO password_reset_tokens (user_id, token, expires_at) VALUES (@userId, @token, @expiresAt)";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Parameters.AddWithValue("@token", token);
        cmd.Parameters.AddWithValue("@expiresAt", expiresAt);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<(int UserId, bool Used, DateTime ExpiresAt)?> GetToken(string token)
    {
        using var conn = await _dbConnectie.GetConnection();
        var sql = "SELECT user_id, used, expires_at FROM password_reset_tokens WHERE token = @token";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@token", token);
        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return (
                reader.GetInt32(reader.GetOrdinal("user_id")),
                reader.GetBoolean(reader.GetOrdinal("used")),
                reader.GetDateTime(reader.GetOrdinal("expires_at"))
            );
        }

        return null;
    }

    public async Task MarkTokenAsUsed(string token)
    {
        using var conn = await _dbConnectie.GetConnection();
        var sql = "UPDATE password_reset_tokens SET used = TRUE WHERE token = @token";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@token", token);
        await cmd.ExecuteNonQueryAsync();
    }
}
