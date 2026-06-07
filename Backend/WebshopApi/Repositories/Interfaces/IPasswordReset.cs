public interface IPasswordReset
{
    Task SaveToken(int userId, string token, DateTime expiresAt);

    Task<(int UserId, bool Used, DateTime ExpiresAt)?> GetToken(string token);

    Task MarkTokenAsUsed(string token);

}
