using Npgsql;
using System.Threading.Tasks;
using models;
using System.Data.SqlTypes;
using System.Data.Common;
using System.Reflection.Metadata;
using ApplicationDbContext;

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
                Name = reader.GetString(reader.GetOrdinal("name")),
                Email = reader.GetString(reader.GetOrdinal("email"))
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
                Name = reader.GetString(reader.GetOrdinal("name")),
                Email = reader.GetString(reader.GetOrdinal("email"))
            };
        }

        return null;
    }

    public async void AddUser(Users user)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = "INSERT INTO users (name, email) VALUES (@name, @email)";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@name", user.Name);
        cmd.Parameters.AddWithValue("@email", user.Email);
        await cmd.ExecuteNonQueryAsync();
    }

    public async void UpdateUser(Users user)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = "UPDATE users SET name = @name, email = @email WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", user.Id);
        cmd.Parameters.AddWithValue("@name", user.Name);
        cmd.Parameters.AddWithValue("@email", user.Email);
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