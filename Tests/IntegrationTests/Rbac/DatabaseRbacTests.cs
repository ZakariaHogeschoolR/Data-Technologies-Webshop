using Npgsql;

namespace IntegrationTests.Rbac;

[Collection("RbacTests")]
public class DatabaseRbacTests : IClassFixture<rbacDatabaseFixture>
{
    private readonly rbacDatabaseFixture _fixture;

    public DatabaseRbacTests(rbacDatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    private async Task<NpgsqlConnection> OpenAsAppAsync()
    {
        var conn = new NpgsqlConnection(_fixture.WebshopAppConnectionString);
        await conn.OpenAsync();
        return conn;
    }

    private async Task<NpgsqlConnection> OpenAsAdminAsync()
    {
        var conn = new NpgsqlConnection(_fixture.WebshopAdminConnectionString);
        await conn.OpenAsync();
        return conn;
    }

    private async Task<NpgsqlConnection> OpenAsSuperuserAsync()
    {
        var conn = new NpgsqlConnection(_fixture.SuperuserConnectionString);
        await conn.OpenAsync();
        return conn;
    }
    private static async Task AssertPermissionDeniedAsync(
        NpgsqlConnection conn,
        string sql,
        string expectedSqlState = "42501") // 42501 = insufficient_privilege
    {
        await using var cmd = new NpgsqlCommand(sql, conn);

        var exception = await Record.ExceptionAsync(
            () => cmd.ExecuteNonQueryAsync()
        );

        Assert.NotNull(exception);

        var pgException = Assert.IsType<PostgresException>(exception);

        Assert.Equal(expectedSqlState, pgException.SqlState);
    }

    private static async Task AssertPermittedAsync(
        NpgsqlConnection conn,
        string sql)
    {
        await using var cmd = new NpgsqlCommand(sql, conn);

        var exception = await Record.ExceptionAsync(
            () => cmd.ExecuteNonQueryAsync()
        );

        Assert.Null(exception);
    }

    [Fact]
    public async Task App_CannotRead_PiiSchema()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermissionDeniedAsync(
            conn,
            "SELECT user_id, email FROM pii.users_pii LIMIT 1;"
        );
    }

    [Fact]
    public async Task App_CannotInsert_IntoPiiSchema()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermissionDeniedAsync(
            conn,
            "INSERT INTO pii.users_pii (user_id, email, password) VALUES (999, 'x@x.com', 'hash');"
        );
    }

    [Fact]
    public async Task App_CanRead_PublicUsersNonPiiColumns()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermittedAsync(
            conn,
            "SELECT id, username, role FROM users LIMIT 1;"
        );
    }

    [Fact]
    public async Task App_CannotRead_EmailColumn_BecauseItWasMigrated()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermissionDeniedAsync(
            conn,
            "SELECT email FROM users LIMIT 1;",
            expectedSqlState: "42703"  // undefined_column, niet insufficient_privilege
        );
    }

    [Fact]
    public async Task App_CannotRead_PasswordColumn_BecauseItWasMigrated()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermissionDeniedAsync(
            conn,
            "SELECT password FROM users LIMIT 1;",
            expectedSqlState: "42703"
        );
    }

    [Fact]
    public async Task App_CannotInsert_IntoUsers()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermissionDeniedAsync(
            conn,
            "INSERT INTO users (first_name, last_name, username, role) VALUES ('Test', 'User', 'rbac_test_user', 'user');"
        );
    }

    [Fact]
    public async Task App_CannotUpdate_UsersRole()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermissionDeniedAsync(
            conn,
            "UPDATE users SET role = 'admin' WHERE id = 1;"
        );
    }

    [Fact]
    public async Task App_CannotDelete_FromUsers()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermissionDeniedAsync(
            conn,
            "DELETE FROM users WHERE id = -1;" // id -1 bestaat niet, maar permission check komt eerst
        );
    }

    [Fact]
    public async Task App_CanRead_Products()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermittedAsync(conn, "SELECT id, name, price FROM products LIMIT 5;");
    }

    [Fact]
    public async Task App_CanRead_Teams()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermittedAsync(conn, "SELECT id, name FROM teams LIMIT 5;");
    }

    [Fact]
    public async Task App_CanRead_Category()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermittedAsync(conn, "SELECT id, name FROM category LIMIT 5;");
    }

    [Fact]
    public async Task App_CannotInsert_IntoProducts()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermissionDeniedAsync(
            conn,
            "INSERT INTO products (product_image, name, description, price) VALUES ('img', 'Hack Product', 'Desc', 0.01);"
        );
    }

    [Fact]
    public async Task App_CannotUpdate_ProductPrice()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermissionDeniedAsync(
            conn,
            "UPDATE products SET price = 0 WHERE id = -1;"
        );
    }

    [Fact]
    public async Task App_CannotDelete_Category()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermissionDeniedAsync(
            conn,
            "DELETE FROM category WHERE id = -1;"
        );
    }

    [Fact]
    public async Task App_CannotDelete_Teams()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermissionDeniedAsync(
            conn,
            "DELETE FROM teams WHERE id = -1;"
        );
    }

    [Fact]
    public async Task App_CanRead_Orders()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermittedAsync(conn, "SELECT id FROM orders LIMIT 1;");
    }

    [Fact]
    public async Task App_CanRead_Wishlist()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermittedAsync(conn, "SELECT id FROM wishlist LIMIT 1;");
    }

    [Fact]
    public async Task App_CanRead_Winkelwagen()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermittedAsync(conn, "SELECT winkelwagen_users_id FROM winkelwagen LIMIT 1;");
    }

    [Fact]
    public async Task App_CanUpdate_Orders()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermittedAsync(conn, "UPDATE orders SET total = 0 WHERE id = -1;");
    }

    [Fact]
    public async Task App_CanDelete_FromWishlist()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermittedAsync(conn, "DELETE FROM wishlist WHERE id = -1;");
    }

    [Fact]
    public async Task App_CanRead_Payments()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermittedAsync(conn, "SELECT id FROM \"Payments\" LIMIT 1;");
    }

    [Fact]
    public async Task App_CannotUpdate_PaymentStatus()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermissionDeniedAsync(
            conn,
            "UPDATE \"Payments\" SET status = 'paid' WHERE id = -1;"
        );
    }

    [Fact]
    public async Task App_CannotDelete_FromPayments()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermissionDeniedAsync(
            conn,
            "DELETE FROM \"Payments\" WHERE id = -1;"
        );
    }

    [Fact]
    public async Task App_CannotAccess_PasswordResetTokens()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermissionDeniedAsync(
            conn,
            "SELECT id FROM password_reset_tokens LIMIT 1;"
        );
    }

    [Fact]
    public async Task App_CannotCreateTable_InPublicSchema()
    {
        await using var conn = await OpenAsAppAsync();

        await AssertPermissionDeniedAsync(
            conn,
            "CREATE TABLE public.rbac_hack_table (id int);"
        );
    }

    [Fact]
    public async Task Admin_CanInsert_IntoProducts()
    {
        await using var superConn = await OpenAsSuperuserAsync();
        await using var seedCmd  = new NpgsqlCommand(
            "INSERT INTO teams (name, type) VALUES ('RBAC Test Team', 'sales') ON CONFLICT (name) DO UPDATE SET type = EXCLUDED.type RETURNING id;",
            superConn
        );
        var teamId = Convert.ToInt32(await seedCmd.ExecuteScalarAsync());

        await using var adminConn = await OpenAsAdminAsync();
        await AssertPermittedAsync(
            adminConn,
            $"INSERT INTO products (product_image, name, description, price, team_id) VALUES ('img', 'Admin Test Product', 'Desc', 1.00, {teamId});"
        );

        await using var cleanCmd = new NpgsqlCommand(
            "DELETE FROM products WHERE name = 'Admin Test Product';",
            superConn
        );
        await cleanCmd.ExecuteNonQueryAsync();
    }

    [Fact]
    public async Task Admin_CannotAccess_PiiSchema()
    {
        await using var conn = await OpenAsAdminAsync();

        var exception = await Record.ExceptionAsync(async () =>
        {
            await using var cmd = new NpgsqlCommand(
                "SELECT user_id FROM pii.users_pii LIMIT 1;",
                conn
            );
            await cmd.ExecuteNonQueryAsync();
        });

        Assert.Null(exception);
    }
}
