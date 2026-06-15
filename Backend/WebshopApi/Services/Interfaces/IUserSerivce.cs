using DataTransferObject;

using models;

public interface IUserService
{
    Task<List<Users>> GetAllService();

    Task<Users> GetByIdService(int id);

    Task CreateService(UserDto user);

    Task UpdateService(UserDto user);

    Task ResetPasswordService(int id, string newPassword);

    Task UpdateRoleService(int id, string role);

    Task DeleteService(int id);

    Task RegisterService(Register data);

    Task<Users?> LoginService(Login data);

    Task FollowUser(string userId, string targetUserId);

    Task<List<Products>> GetRecommendation(int userId);
}
