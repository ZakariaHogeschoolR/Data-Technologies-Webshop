using DataTransferObject;

using models;

public interface IUserGraph
{
    Task FollowUser(string userId, string targetUserId);

    Task<List<Products>> GetRecommendation(int userId);
}