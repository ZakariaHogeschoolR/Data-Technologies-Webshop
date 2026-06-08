using System.Data.Common;
using System.Data.SqlTypes;
using System.Reflection.Metadata;
using System.Threading.Tasks;

using ApplicationDbContext;

using DataTransferObject;

using models;

using Npgsql;

public class UserRepository : IUser
{
    private readonly DatabaseConnectie _dbConnectie;
    private readonly Neo4jService _neo4j;

    public UserRepository(DatabaseConnectie dbConnectie, Neo4jService neo4j)
    {
        _dbConnectie = dbConnectie;
        _neo4j = neo4j;
    }

    public async Task<List<Users?>> GetAllUsers()
    {
        var users = new List<Users>();
        using var conn = await _dbConnectie.GetConnection();
        var sql = "SELECT * FROM users";

        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            users.Add(new Users
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                LastName = reader.GetString(reader.GetOrdinal("last_name")),
                Username = reader.GetString(reader.GetOrdinal("username")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                Password = reader.GetString(reader.GetOrdinal("password")),
                Address = reader.GetString(reader.GetOrdinal("address")),
                PostCode = reader.GetString(reader.GetOrdinal("postcode")),
                Role = reader.GetString(reader.GetOrdinal("role")),
            });
        }
        // only activate when there is already items in the database
        //await GetAllUsersForGraph();
        return users;
    }

    public async Task<List<Users?>> GetAllUsersForGraph()
    {
        // this is method should only be used once everytime we delete the entity relational diagram.
        // and even than it should still not be used cause addproduct does the same thing but just for one singular User.
        // this method is only usefull if there are already users in the database.
        var users = new List<Users>();
        using var conn = await _dbConnectie.GetConnection();
        var sql = "SELECT * FROM users";

        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            users.Add(new Users
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                LastName = reader.GetString(reader.GetOrdinal("last_name")),
                Username = reader.GetString(reader.GetOrdinal("username")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                Password = reader.GetString(reader.GetOrdinal("password")),
                Address = reader.GetString(reader.GetOrdinal("address")),
                PostCode = reader.GetString(reader.GetOrdinal("postcode")),
                Role = reader.GetString(reader.GetOrdinal("role")),
            });
        }

        foreach (Users user in users)
        {
            await AddUserToGraph(user);
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
                PostCode = reader.GetString(reader.GetOrdinal("postcode")),
                Role = reader.GetString(reader.GetOrdinal("role")),
            };
        }

        return null;
    }

    public async Task<Users?> GetUserByEmail(string email)
    {
        await using var conn = await _dbConnectie.GetConnection();

        const string sql = "SELECT * FROM users WHERE email = @email";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@email", email);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (!await reader.ReadAsync()) return null;

        return new Users
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            FirstName = reader.GetString(reader.GetOrdinal("first_name")),
            LastName = reader.GetString(reader.GetOrdinal("last_name")),
            Username = reader.GetString(reader.GetOrdinal("username")),
            Email = reader.GetString(reader.GetOrdinal("email")),
            Password = reader.GetString(reader.GetOrdinal("password")),
            Address = reader.GetString(reader.GetOrdinal("address")),
            PostCode = reader.GetString(reader.GetOrdinal("postcode")),
            Role = reader.GetString(reader.GetOrdinal("role")),
        };
    }

    public async Task AddUser(UserDto user)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = "INSERT INTO users (first_name, last_name, username, email, password, address, postcode, role) VALUES (@firstName, @lastName, @username, @email, @password, @address, @postcode, @role)";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@firstName", user.FirstName);
        cmd.Parameters.AddWithValue("@lastName", user.LastName);
        cmd.Parameters.AddWithValue("@username", user.Username);
        cmd.Parameters.AddWithValue("@email", user.Email);
        cmd.Parameters.AddWithValue("@password", user.Password);
        cmd.Parameters.AddWithValue("@address", user.Address);
        cmd.Parameters.AddWithValue("@postcode", user.PostCode);
        cmd.Parameters.AddWithValue("@role", "user");
        await cmd.ExecuteNonQueryAsync();
        Users userByEmail = await GetUserByEmail(user.Email);
        await AddUserToGraph(user, userByEmail.Id);
    }

    private async Task AddUserToGraph(UserDto user, int userId)
    {
        await using var session = _neo4j.CreateSession();

        var query = @"
            MERGE (u:User {id: $id})
            SET u.firstName = $firstName,
                u.lastName = $lastName,
                u.userName = $userName,
                u.email = $email,
                u.password = $password,
                u.address = $address,
                u.postCode = $postCode
        ";

        await session.RunAsync(query, new
        {
            id = userId,
            firstName = user.FirstName,
            lastName = user.LastName,
            userName = user.Username,
            email = user.Email,
            password = user.Password,
            address = user.Address,
            postCode = user.PostCode

        });
    }

    private async Task AddUserToGraph(Users user)
    {
        await using var session = _neo4j.CreateSession();

        var query = @"
            MERGE (u:User {id: $id})
            SET u.firstName = $firstName,
                u.lastName = $lastName,
                u.userName = $userName,
                u.email = $email,
                u.password = $password,
                u.address = $address,
                u.postCode = $postCode
        ";

        await session.RunAsync(query, new
        {
            id = user.Id,
            firstName = user.FirstName,
            lastName = user.LastName,
            userName = user.Username,
            email = user.Email,
            password = user.Password,
            address = user.Address,
            postCode = user.PostCode
        });
    }

    public async Task UpdateUser(UserDto user)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = "UPDATE users SET first_name = @firstName, last_name = @lastName, username = @username, email = @email, password = @password, address = @address, postcode = @postcode WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@firstName", user.FirstName);
        cmd.Parameters.AddWithValue("@lastName", user.LastName);
        cmd.Parameters.AddWithValue("@username", user.Username);
        cmd.Parameters.AddWithValue("@email", user.Email);
        cmd.Parameters.AddWithValue("@password", user.Password);
        cmd.Parameters.AddWithValue("@address", user.Address);
        cmd.Parameters.AddWithValue("@postcode", user.PostCode);
        cmd.Parameters.AddWithValue("@id", user.Id);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdatePassword(int id, string hashedPassword)
    {
        using var conn = await _dbConnectie.GetConnection();
        var sql = "UPDATE users SET password = @password WHERE id = @id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@password", hashedPassword);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task UpdateRole(int id, string role)
    {
        using var conn = await _dbConnectie.GetConnection();
        var sql = "UPDATE users SET role = @role WHERE id = @id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@role", role);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteUser(int id)
    {
        using var conn = await _dbConnectie.GetConnection();

        var sql = "DELETE FROM users WHERE id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();
    }
}
