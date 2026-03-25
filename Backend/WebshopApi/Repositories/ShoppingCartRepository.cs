using ApplicationDbContext;
using models;
using Npgsql;
using DataTransferObject;

public class ShoppingCartRepository
{
    private readonly DatabaseConnectie _dbconnectie;
    public ShoppingCartRepository(DatabaseConnectie dbconnectie)
    {
        _dbconnectie = dbconnectie;
    }
    public async Task<List<ShoppingCarts?>> GetAllShoppingCarts()
    {
        List<ShoppingCarts> shoppingcarts = new List<ShoppingCarts>();
        using var conn = await _dbconnectie.GetConnection();
        var sql = "SELECT * FROM winkelwagen";
        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while(await reader.ReadAsync())
        {
            shoppingcarts.Add(new ShoppingCarts
            {
                Id = reader.GetInt32(reader.GetOrdinal("winkelwagen_users_id")),
                ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                Quantity = reader.GetInt32(reader.GetOrdinal("quantity"))
            });
        }

        return shoppingcarts;
    }

    public async Task<List<WinkelwagenUser>> GetAllWinkelwagenUsers()
    {
        List<WinkelwagenUser> users = new List<WinkelwagenUser>();
        using var conn = await _dbconnectie.GetConnection();
        var sql = "SELECT * FROM winkelwagen_users";
        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while(await reader.ReadAsync())
        {
            users.Add(new WinkelwagenUser
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                UserId = reader.GetInt32(reader.GetOrdinal("user_id")),
            });
        }

        return users;
    }

    public async Task<ShoppingCarts?> GetShoppingCartById(int id)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "SELECT * FROM winkelwagen WHERE winkelwagen_users_id = @id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = await cmd.ExecuteReaderAsync();

        if(await reader.ReadAsync())
        {
            return new ShoppingCarts
            {
                Id = reader.GetInt32(reader.GetOrdinal("winkelwagen_users_id")),
                ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                Quantity = reader.GetInt32(reader.GetOrdinal("quantity"))
            };
        }

        return null;
    }
    public async Task AddShoppingCarts(ShoppingCartDTO shoppingcarts)
    {
        using var conn = await _dbconnectie.GetConnection();
        var cmd = new NpgsqlCommand(@"INSERT INTO winkelwagen_users (user_id, created_at) 
        VALUES (@U_ID, @CR_AT) ON CONFLICT (user_id) 
        DO UPDATE SET user_id = EXCLUDED.user_id RETURNING id", conn);
        cmd.Parameters.AddWithValue("@U_ID", shoppingcarts.UserId);
        cmd.Parameters.AddWithValue("@CR_AT", DateTime.UtcNow);
        var internalId = await cmd.ExecuteScalarAsync();
        var cmd1 = new NpgsqlCommand(@"INSERT INTO winkelwagen (winkelwagen_users_id) 
        VALUES (@U_ID)", conn);
        cmd1.Parameters.AddWithValue("U_ID", internalId);
        await cmd1.ExecuteNonQueryAsync();
    }
    //in between table
    public async void AddProduct(Products product, int quantity)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "UPDATE winkelwagen (product_id, quantity) VALUES (@id, @quantity) WHERE user_id= @U_ID";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", product.Id);
        cmd.Parameters.AddWithValue("@id", quantity);
        await cmd.ExecuteNonQueryAsync();
    }
    public async void UpdateShoppingcarts(int id, int quantity)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "UPDATE winkelwagen SET (product_id, quantity) VALUES(@id,@quantity)";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@quantity", quantity);
        await cmd.ExecuteNonQueryAsync();
    }
    public async void DeleteShoppingCarts(int id)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "DELETE * FROM winkelwagen SELECT * WHERE (winkelwagen_users_id) VALUES (@id)";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();
    }
    public async void DeleteProductFromShoppingcarts(int userid, int id)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "DELETE * FROM winkelwagen WHERE (product_id, winkelwagen_users_id) VALUES (@id,@userid)";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        cmd.Parameters.AddWithValue("@userid", userid);
        await cmd.ExecuteNonQueryAsync();
    }
}