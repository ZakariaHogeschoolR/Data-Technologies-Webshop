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

    public async Task<List<ShoppingCarts?>> GetShoppingCartById(int id)
    {
        var shoppingcartslist = new List<ShoppingCarts>();
        using var conn = await _dbconnectie.GetConnection();
        
        var sql = @"SELECT w.winkelwagen_users_id, w.product_id,
        w.quantity, wu.created_at
        FROM winkelwagen w 
        JOIN winkelwagen_users wu 
        ON w.winkelwagen_users_id = wu.id 
        WHERE wu.user_id = @id";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = await cmd.ExecuteReaderAsync();

        while(await reader.ReadAsync())
        {
            shoppingcartslist.Add(new ShoppingCarts
            {
                Id = reader.GetInt32(reader.GetOrdinal("winkelwagen_users_id")),
                ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                CreatedAt = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("created_at")))
            });
        }

        return shoppingcartslist;
    }
    public async Task<ShoppingCarts> AddShoppingCarts(ShoppingCartDTO shoppingcarts)
    {
        using var conn = await _dbconnectie.GetConnection();
        using var transaction = await conn.BeginTransactionAsync();
        try
        {
            var cmd = new NpgsqlCommand(@"INSERT INTO winkelwagen_users (user_id, created_at) 
            VALUES (@U_ID, @CR_AT) ON CONFLICT (user_id) DO UPDATE SET user_id = EXCLUDED.user_id RETURNING id, created_at", conn);
            
            cmd.Parameters.AddWithValue("@U_ID", shoppingcarts.UserId);
            cmd.Parameters.AddWithValue("@CR_AT", DateTime.UtcNow);
            
            using var reader = await cmd.ExecuteReaderAsync();
            if(!await reader.ReadAsync()) throw new Exception("winkelwagen_user kon niet gemaakt worden");
            var WUid = reader.GetInt32(reader.GetOrdinal("id"));
            var createdAt =reader.GetDateTime(reader.GetOrdinal("created_at"));
            await reader.CloseAsync();
            // var newWUID = Convert.ToInt32(result);
            
            var cmd1 = new NpgsqlCommand(@"INSERT INTO winkelwagen 
            (winkelwagen_users_id, product_id, quantity) 
            VALUES (@WU_ID, @P_ID, @QUAN)
            ON CONFLICT (winkelwagen_users_id, product_id)
            DO UPDATE SET quantity = winkelwagen.quantity + EXCLUDED.quantity
            RETURNING winkelwagen_users_id, quantity", conn);
            
            cmd1.Parameters.AddWithValue("WU_ID", WUid);
            cmd1.Parameters.AddWithValue("P_ID", shoppingcarts.ProductId);
            cmd1.Parameters.AddWithValue("QUAN", shoppingcarts.Quantity);

            using var reader1 = await cmd1.ExecuteReaderAsync();
            if(!await reader1.ReadAsync()) throw new Exception("kon product niet toevoegen aan winkelwagen");
            
            var Wid = reader1.GetInt32(reader1.GetOrdinal("winkelwagen_users_id"));
            var quantity = reader1.GetInt32(reader1.GetOrdinal("quantity"));
            await reader1.CloseAsync();
            
            await transaction.CommitAsync();
            
            return new ShoppingCarts
            {
                Id = Wid,
                ProductId = shoppingcarts.ProductId,
                Quantity = quantity,
                CreatedAt = DateOnly.FromDateTime(createdAt),
                UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
            };
            
        }
        catch(Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception("Error in making winkelwagen: " + ex.Message);
        }
    }
    //in between table
    // public async void ChangeQuantity(Products product, int quantity)
    // {
    //     using var conn = await _dbconnectie.GetConnection();
    //     var sql = "UPDATE winkelwagen (quantity) VALUES (@QUAN) WHERE winkelwagen_users_id= @WU_ID AND product_id= @P_ID";
    //     using var cmd = new NpgsqlCommand(sql, conn);
    //     cmd.Parameters.AddWithValue("@WU_ID", );
    //     cmd.Parameters.AddWithValue("@P_OD", product.Id);
    //     cmd.Parameters.AddWithValue("@QUAN", quantity);
    //     await cmd.ExecuteNonQueryAsync();
    // }
    public async void UpdateShoppingcarts(ShoppingCartDTO shoppingCartDTO)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "UPDATE winkelwagen SET product_id=@P_ID, quantity=@QUAN WHERE winkelwagen_users_id=@WU_ID";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@WU_ID", shoppingCartDTO.Id);
        cmd.Parameters.AddWithValue("@P_ID", shoppingCartDTO.ProductId);
        cmd.Parameters.AddWithValue("@QUAN", shoppingCartDTO.Quantity);
        await cmd.ExecuteNonQueryAsync();
    }
    public async Task DeleteShoppingCarts(int id)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "DELETE FROM winkelwagen WHERE winkelwagen_users_id =@id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();

        var sql1 = "DELETE FROM winkelwagen_users WHERE user_id =@id";
        using var cmd1 = new NpgsqlCommand(sql1, conn);
        cmd1.Parameters.AddWithValue("@id", id);
        await cmd1.ExecuteNonQueryAsync();
    }
    public async void DeleteProductFromShoppingcarts(ShoppingCartDTO shoppingCartDTO)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "DELETE * FROM winkelwagen WHERE (product_id, winkelwagen_users_id) VALUES (@P_ID, @U_ID)";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@P_ID", shoppingCartDTO.ProductId);
        cmd.Parameters.AddWithValue("@U_ID", shoppingCartDTO.UserId);
        await cmd.ExecuteNonQueryAsync();
    }
}