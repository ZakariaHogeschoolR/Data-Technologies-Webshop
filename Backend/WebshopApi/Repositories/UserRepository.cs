using Npgsql;
using System.Threading.Tasks;
using models;
using System.Data.SqlTypes;
using System.Data.Common;
using System.Reflection.Metadata;
using ApplicationDbContext;
using DataTransferObject;

public class UserRepository
{
    private readonly DatabaseConnectie _dbConnectie;

    public UserRepository(DatabaseConnectie dbConnectie)
    {
        _dbConnectie = dbConnectie;
    }

    public async Task<List<Users?>> GetAllUsers()
    {
        var users = new List<Users>();
        using var conn = await _dbConnectie.GetConnection();
        var sql = "SELECT * FROM users";

        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while(await reader.ReadAsync())
        {
            users.Add( new Users
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                LastName = reader.GetString(reader.GetOrdinal("last_name")),
                Username = reader.GetString(reader.GetOrdinal("username")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                Password = reader.GetString(reader.GetOrdinal("password")),
                Address = reader.GetString(reader.GetOrdinal("address")),
                PostCode = reader.GetString(reader.GetOrdinal("post_code")),
            });
        }

        return users;
    }

    public async Task<Users?> GetUserById(int id)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = "SELECT * FROM users WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Users
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                LastName = reader.GetString(reader.GetOrdinal("last_name")),
                Username = reader.GetString(reader.GetOrdinal("username")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                Password = reader.GetString(reader.GetOrdinal("password")),
                Address = reader.GetString(reader.GetOrdinal("address")),
                PostCode = reader.GetString(reader.GetOrdinal("post_code")),
            };
        }

        return null;
    }

    public async void AddUser(UserDto user)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = "INSERT INTO users (first_name, last_name, username, email, password, address, post_code) VALUES (@firstName, @lastName, @username, @email, @password, @address, @postcode)";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@firstName", user.FirstName);
        cmd.Parameters.AddWithValue("@lastName", user.LastName);
        cmd.Parameters.AddWithValue("@username", user.Username);
        cmd.Parameters.AddWithValue("@email", user.Email);
        cmd.Parameters.AddWithValue("@password", user.Password);
        cmd.Parameters.AddWithValue("@address", user.Address);
        cmd.Parameters.AddWithValue("@postcode", user.PostCode);
        await cmd.ExecuteNonQueryAsync();
    }

    public async void UpdateUser(UserDto user)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = "UPDATE users SET first_name = @firstName, last_name = lastName, username = @username, email = @email, password = @password, address = @address, postcode = @postcode WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@firstName", user.FirstName);
        cmd.Parameters.AddWithValue("@lastName", user.LastName);
        cmd.Parameters.AddWithValue("@username", user.Username);
        cmd.Parameters.AddWithValue("@email", user.Email);
        cmd.Parameters.AddWithValue("@password", user.Password);
        cmd.Parameters.AddWithValue("@address", user.Address);
        cmd.Parameters.AddWithValue("@postcode", user.PostCode);
        await cmd.ExecuteNonQueryAsync();
    }

    public async void DeleteUser(int id)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = "DELETE FROM users WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}