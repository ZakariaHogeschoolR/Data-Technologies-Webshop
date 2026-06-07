using System.Data.Common;
using System.Data.SqlTypes;
using System.Reflection.Metadata;
using System.Threading.Tasks;

using ApplicationDbContext;

using DataTransferObject;

using models;

using Npgsql;

public class UserRepository
{
    private readonly AdminDatabaseConnection _dbConnectie;
    private readonly Neo4jService _neo4j;

    public UserRepository(AdminDatabaseConnection dbConnectie, Neo4jService neo4j)
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
        await using var conn = await _dbConnectie.GetConnection();

        const string sql = """
                           SELECT u.id, u.first_name, u.last_name, u.username, u.role,
                                  p.email, p.password, p.address, p.postcode
                           FROM public.users u
                           JOIN pii.users_pii p ON p.user_id = u.id
                           WHERE u.id = @id
                           """;

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        await using var reader = await cmd.ExecuteReaderAsync();

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

        const string sql = """
                           SELECT u.id, u.first_name, u.last_name, u.username, u.role,
                                  p.email, p.password, p.address, p.postcode
                           FROM pii.users_pii p
                           JOIN public.users u ON u.id = p.user_id
                           WHERE p.email = @email
                           """;

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
        await using var conn = await _dbConnectie.GetConnection();
        await using var transaction = await conn.BeginTransactionAsync();

        try
        {
            const string coreSql = """
                                   INSERT INTO public.users (first_name, last_name, username, role)
                                   VALUES (@firstName, @lastName, @username, @role)
                                   RETURNING id
                                   """;
            await using var coreCmd = new NpgsqlCommand(coreSql, conn, transaction);
            coreCmd.Parameters.AddWithValue("@firstName", user.FirstName);
            coreCmd.Parameters.AddWithValue("@lastName", user.LastName);
            coreCmd.Parameters.AddWithValue("@username", user.Username);
            coreCmd.Parameters.AddWithValue("@role", "user");
            var userId = (int)(await coreCmd.ExecuteScalarAsync())!;

            const string piiSql = """
                                  INSERT INTO pii.users_pii (user_id, email, password, address, postcode)
                                  VALUES (@userId, @email, @password, @address, @postcode)
                                  """;
            await using var piiCmd = new NpgsqlCommand(piiSql, conn, transaction);
            piiCmd.Parameters.AddWithValue("@userId", userId);
            piiCmd.Parameters.AddWithValue("@email", user.Email);
            piiCmd.Parameters.AddWithValue("@password", user.Password);
            piiCmd.Parameters.AddWithValue("@address", user.Address);
            piiCmd.Parameters.AddWithValue("@postcode", user.PostCode);
            await piiCmd.ExecuteNonQueryAsync();

            await transaction.CommitAsync();

            await AddUserToGraph(user, userId);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
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
        await using var conn = await _dbConnectie.GetConnection();
        await using var transaction = await conn.BeginTransactionAsync();

        try
        {
            const string coreSql = """
                                   UPDATE public.users SET first_name = @firstName, last_name = @lastName, username = @username
                                   WHERE id = @id
                                   """;

            await using var coreCmd = new NpgsqlCommand(coreSql, conn, transaction);
            coreCmd.Parameters.AddWithValue("@firstName", user.FirstName);
            coreCmd.Parameters.AddWithValue("@lastName", user.LastName);
            coreCmd.Parameters.AddWithValue("@username", user.Username);
            coreCmd.Parameters.AddWithValue("@id", user.Id);

            await coreCmd.ExecuteNonQueryAsync();

            const string piiSql = """
                                  UPDATE pii.users_pii SET email = @email, password = @password, address = @address, postcode = @postcode
                                  WHERE user_id = @userId
                                  """;

            await using var piiCmd = new NpgsqlCommand(piiSql, conn, transaction);
            piiCmd.Parameters.AddWithValue("@userId", user.Id);
            piiCmd.Parameters.AddWithValue("@email", user.Email);
            piiCmd.Parameters.AddWithValue("@password", user.Password);
            piiCmd.Parameters.AddWithValue("@address", user.Address);
            piiCmd.Parameters.AddWithValue("@postcode", user.PostCode);

            await piiCmd.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task UpdatePassword(int id, string hashedPassword)
    {
        await using var conn = await _dbConnectie.GetConnection();
        const string sql = "UPDATE pii.users_pii SET password = @password WHERE user_id = @id";
        await using var cmd = new NpgsqlCommand(sql, conn);
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
