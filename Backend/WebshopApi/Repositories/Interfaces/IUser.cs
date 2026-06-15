using DataTransferObject;

using models;

public interface IUser
{
    Task<List<Users?>> GetAllUsers();

    Task<List<Users?>> GetAllUsersForGraph();

    Task<Users?> GetUserById(int id);

    Task<Users?> GetUserByEmail(string email);

    Task AddUser(UserDto user);

    Task UpdateUser(UserDto user);

    Task UpdatePassword(int id, string hashedPassword);

    Task UpdateRole(int id, string role);

    Task DeleteUser(int id);
}
