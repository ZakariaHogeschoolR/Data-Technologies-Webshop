using ApplicationDbContext;

using DataTransferObject;

using models;

using Npgsql;

public class ShoppingCartRepository
{
    private readonly DatabaseConnectie _dbconnectie;

    public ShoppingCartRepository(DatabaseConnectie dbconnectie)
    {
        _dbconnectie = dbconnectie;
    }

    public async Task<List<ShoppingCarts?>> GetAllShoppingCarts()
    {
        var shoppingcarts = new List<ShoppingCarts>();
        using var conn = await _dbconnectie.GetConnection();
        var sql = "SELECT * FROM winkelwagen";
        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            shoppingcarts.Add(new ShoppingCarts
            {
                Id = reader.GetInt32(reader.GetOrdinal("winkelwagen_users_id")),
                ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                Quantity = reader.GetInt32(reader.GetOrdinal("quantity"))
            });

        return shoppingcarts;
    }

    public async Task<List<WinkelwagenUser>> GetAllWinkelwagenUsers()
    {
        var users = new List<WinkelwagenUser>();
        using var conn = await _dbconnectie.GetConnection();
        var sql = "SELECT * FROM winkelwagen_users";
        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            users.Add(new WinkelwagenUser
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                UserId = reader.GetInt32(reader.GetOrdinal("user_id"))
            });

        return users;
    }

    public async Task<List<ShoppingCarts?>> GetShoppingCartById(int id)
    {
        var shoppingCartList = new List<ShoppingCarts?>();
        await using var conn = await _dbconnectie.GetConnection();

        const string sql = "SELECT * FROM cart_details WHERE user_id = @id";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            shoppingCartList.Add(new ShoppingCarts
            {
                Id = reader.GetInt32(reader.GetOrdinal("winkelwagen_users_id")),
                ProductId = reader.GetInt32(reader.GetOrdinal("product_id")),
                Quantity = reader.GetInt32(reader.GetOrdinal("quantity")),
                CreatedAt = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("created_at")))
            });

        return shoppingCartList;
    }

    public async Task<ShoppingCarts> AddShoppingCarts(ShoppingCartDTO shoppingcarts)
    {
        using var conn = await _dbconnectie.GetConnection();
        using var transaction = await conn.BeginTransactionAsync();
        try
        {
            var cmd = new NpgsqlCommand(@"INSERT INTO winkelwagen_users (user_id, created_at)
            VALUES (@U_ID, @CR_AT) ON CONFLICT (user_id) DO UPDATE SET user_id = EXCLUDED.user_id RETURNING id, created_at",
                conn);

            cmd.Parameters.AddWithValue("@U_ID", shoppingcarts.UserId);
            cmd.Parameters.AddWithValue("@CR_AT", DateTime.UtcNow);

            using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync()) throw new Exception("winkelwagen_user kon niet gemaakt worden");
            var WUid = reader.GetInt32(reader.GetOrdinal("id"));
            var createdAt = reader.GetDateTime(reader.GetOrdinal("created_at"));
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
            if (!await reader1.ReadAsync()) throw new Exception("kon product niet toevoegen aan winkelwagen");

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
                UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            };
        }
        catch (Exception ex)
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
    public async Task DeleteProductFromShoppingcarts(ShoppingCartDTO shoppingCartDTO)
    {
        using var conn = await _dbconnectie.GetConnection();
        using var transaction = await conn.BeginTransactionAsync();
        try
        {
            var getWWU_ID = new NpgsqlCommand("SELECT id FROM winkelwagen_users WHERE user_id= @U_ID", conn, transaction);
            getWWU_ID.Parameters.AddWithValue("U_ID", shoppingCartDTO.Id);
            var result = await getWWU_ID.ExecuteScalarAsync();
            if (result == null)
            {
                return;
            }
            int WWUID = (int)result;
            var delete = new NpgsqlCommand(@"DELETE FROM winkelwagen WHERE
            winkelwagen_users_id = @WWUID AND product_id = @P_ID", conn, transaction);
            delete.Parameters.AddWithValue("@WWUID", WWUID);
            delete.Parameters.AddWithValue("@P_ID", shoppingCartDTO.ProductId);

            await delete.ExecuteNonQueryAsync();

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception("Verwijder fout by winkelwagen: " + ex.Message);
        }
    }

    public async Task<List<OrderHistoryDto>?> GetOrderHistoryByUserId(int userId)
    {
        await using var conn = await _dbconnectie.GetConnection();

        const string sql = """
                           WITH cart_history AS (
                               SELECT wu.id AS order_id, wu.created_at AS order_date, w.product_id, w.quantity
                               FROM winkelwagen_users wu
                               JOIN winkelwagen w ON w.winkelwagen_users_id = wu.id
                               WHERE wu.user_id = @userId
                           )
                           SELECT order_id, order_date, product_id, quantity
                           FROM cart_history
                           ORDER BY order_date DESC, product_id
                           """;

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@userId", userId);

        await using var reader = await cmd.ExecuteReaderAsync();

        var orderMap = new Dictionary<int, OrderHistoryDto>();

        while (await reader.ReadAsync())
        {
            var orderId = reader.GetInt32(reader.GetOrdinal("order_id"));

            if (!orderMap.TryGetValue(orderId, out var order))
            {
                order = new OrderHistoryDto(orderId, reader.GetDateTime(reader.GetOrdinal("order_date")), []);
                orderMap[orderId] = order;
            }

            order.Items.Add(new OrderItemDto(reader.GetInt32(reader.GetOrdinal("product_id")),
                reader.GetInt32(reader.GetOrdinal("quantity"))));
        }
        return orderMap.Values.ToList();
    }

    public async Task<CheckoutResultDto> Checkout(int userId, string paymentMethod = "card")
    {
        await using var conn = await _dbconnectie.GetConnection();
        await using var transaction = await conn.BeginTransactionAsync();

        try
        {
            const string fetchSql = """
                                    SELECT wu.id AS wuid, w.product_id, w.quantity, p.name, p.product_image, p.price
                                    FROM winkelwagen_users wu
                                    JOIN winkelwagen w ON w.winkelwagen_users_id = wu.id
                                    JOIN products p ON p.id = w.product_id
                                    WHERE wu.user_id = @userId
                                    """;

            await using var fetchCmd = new NpgsqlCommand(fetchSql, conn, transaction);
            fetchCmd.Parameters.AddWithValue("@userId", userId);

            await using var reader = await fetchCmd.ExecuteReaderAsync();

            var items = new List<CheckoutItemDto>();
            var winkelwagenUsersId = 0;

            while (await reader.ReadAsync())
            {
                winkelwagenUsersId = reader.GetInt32(reader.GetOrdinal("wuid"));
                var price = reader.GetDecimal(reader.GetOrdinal("price"));
                var quantity = reader.GetInt32(reader.GetOrdinal("quantity"));

                items.Add(new CheckoutItemDto(
                    reader.GetInt32(reader.GetOrdinal("product_id")),
                    reader.GetString(reader.GetOrdinal("name")),
                    reader.GetString(reader.GetOrdinal("product_image")),
                    price,
                    quantity,
                    price * quantity
                ));
            }

            await reader.CloseAsync();

            if (items.Count == 0)
                throw new InvalidOperationException("Cart is empty — nothing to checkout.");

            var total = items.Sum(i => i.SubTotal);

            const string deleteOrderSql = "DELETE FROM orders WHERE winkelwagen_users_id = @wuid AND payment_status = FALSE";
            await using var deleteCmd = new NpgsqlCommand(deleteOrderSql, conn, transaction);
            deleteCmd.Parameters.AddWithValue("@wuid", winkelwagenUsersId);
            await deleteCmd.ExecuteNonQueryAsync();

            const string insertOrderSql = """
                                          INSERT INTO orders (winkelwagen_users_id, total, payment_status, created_at)
                                          VALUES (@wuid, @total, FALSE, NOW())
                                          RETURNING id, created_at
                                          """;

            await using var insertCmd = new NpgsqlCommand(insertOrderSql, conn, transaction);
            insertCmd.Parameters.AddWithValue("@wuid", winkelwagenUsersId);
            insertCmd.Parameters.AddWithValue("@total", total);

            await using var orderReader = await insertCmd.ExecuteReaderAsync();
            if (!await orderReader.ReadAsync())
                throw new Exception("Order row was not returned.");

            var orderId = orderReader.GetInt32(orderReader.GetOrdinal("id"));
            var orderedAt = orderReader.GetDateTime(orderReader.GetOrdinal("created_at"));
            await orderReader.CloseAsync();

            const string insertPaymentSql = """
                                            INSERT INTO "Payments" (order_id, paid_at, payment_method, status)
                                            VALUES (@orderId, NOW(), @paymentMethod, 'paid')
                                            """;

            await using var paymentCmd = new NpgsqlCommand(insertPaymentSql, conn, transaction);
            paymentCmd.Parameters.AddWithValue("@orderId", orderId);
            paymentCmd.Parameters.AddWithValue("@paymentMethod", paymentMethod);
            await paymentCmd.ExecuteNonQueryAsync();

            const string updateOrderSql = "UPDATE orders SET payment_status = TRUE WHERE id = @orderId";
            await using var updateCmd = new NpgsqlCommand(updateOrderSql, conn, transaction);
            updateCmd.Parameters.AddWithValue("@orderId", orderId);
            await updateCmd.ExecuteNonQueryAsync();

            const string clearCartSql = "DELETE FROM winkelwagen WHERE winkelwagen_users_id = @wuid";
            await using var clearCmd = new NpgsqlCommand(clearCartSql, conn, transaction);
            clearCmd.Parameters.AddWithValue("@wuid", winkelwagenUsersId);
            await clearCmd.ExecuteNonQueryAsync();

            await transaction.CommitAsync();

            return new CheckoutResultDto(orderId, total, orderedAt, items);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception("Checkout failed. The transaction was rolled back safely.", ex);
        }
    }
}
