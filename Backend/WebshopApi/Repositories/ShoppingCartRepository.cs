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
    public async void AddShoppingCarts(ShoppingCartDTO shoppingcarts)
    {
        await using var batch = new NpgsqlBatch(await _dbconnectie.GetConnection());
        var cmd1 = new NpgsqlBatchCommand("INSERT INTO winkelwagen_users (id, user_id) VALUES (@ID, @USER_ID)");
        cmd1.Parameters.AddWithValue("@ID", shoppingcarts.Id);
        cmd1.Parameters.AddWithValue("@USER_ID", shoppingcarts.User_Id);
        batch.BatchCommands.Add(cmd1);

        var cmd2 = new NpgsqlBatchCommand("INSERT INTO winkelwagen (winkelwagen_users_id, product_id, quantity) VALUES (@winkelwagen_users_id,@product_id, @quantity)");
        cmd2.Parameters.AddWithValue("@winkelwagen_users_id", shoppingcarts.Id);
        cmd2.Parameters.AddWithValue("@product_id", shoppingcarts.ProductId);
        cmd2.Parameters.AddWithValue("@quantity", shoppingcarts.Quantity);

        // {
        //     BatchCommands =
        //     {
        //         new("INSERT INTO winkelwagen_users (id, user_id) VALUES (@ID, @USER_ID)"),
        //         new("INSERT INTO winkelwagen (winkelwagen_users_id, product_id, quantity) VALUES (@winkelwagen_users_id,@product_id, @quantity)")
        //     }
        // };
        
        await using var reader = await batch.ExecuteReaderAsync();

        // using var conn = await _dbconnectie.GetConnection();
        // var sql = "INSERT INTO winkelwagen (winkelwagen_users_id, product_id, quantity) VALUES (@winkelwagen_users_id,@product_id, @quantity)";

        // cmd.Parameters.AddWithValue("@winkelwagen_users_id", shoppingcarts.Id);
        // cmd.Parameters.AddWithValue("@product_id", shoppingcarts.ProductId);
        // cmd.Parameters.AddWithValue("@quantity", shoppingcarts.Quantity);
        // await cmd.ExecuteNonQueryAsync();
    }
    //in between table
    public async void AddProduct(Products product, int quantity)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "INSERT INTO winkelwagen (product_id, quantity) VALUES (@id, @quantity)";
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