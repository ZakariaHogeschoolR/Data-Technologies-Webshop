using ApplicationDbContext;
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
        List<ShoppingCarts> shoppingcarts = new List<ShoppingCarts>();
        using var conn = await _dbconnectie.GetConnection();
        var sql = "SELECT * FROM shoppingcarts";
        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();

        while(await reader.ReadAsync())
        {
            shoppingcarts.Add(new ShoppingCarts
            {
                UserId =  reader.GetOrdinal("userid")
            });
        }

        return shoppingcarts;
    }
    public async Task<ShoppingCarts?> GetShoppingCartById(int id)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "SELECT * FROM shoppingcarts WHERE id = @id";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = await cmd.ExecuteReaderAsync();

        if(await reader.ReadAsync())
        {
            return new ShoppingCarts
            {
                Id = reader.GetInt32(reader.GetOrdinal("id")),
                UserId = reader.GetInt32(reader.GetOrdinal("userid"))
            };
        }

        return null;
    }
    public async void AddShoppingCarts(ShoppingCarts shoppingcarts)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "INSERT INTO shoppingcarts (id,userid) VALUES (@id,@userid)";

        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", shoppingcarts.Id);
        cmd.Parameters.AddWithValue("@userid", shoppingcarts.UserId);
        await cmd.ExecuteNonQueryAsync();
    }
    //in between table
    public async void AddProduct(Products product)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "INSERT INTO shoppingcarts_products (Items) VALUES (@Id,@name)";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", product.Id);
        cmd.Parameters.AddWithValue("@name", product.Name);
        await cmd.ExecuteNonQueryAsync();
    }
    public async void UpdateShoppingcarts(){}
    public async void DeleteShoppingCarts(int id)
    {
        using var conn = await _dbconnectie.GetConnection();
        var sql = "DELETE FROM shoppingcarts SELECT * WHERE (id) VALUES (@id)";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);
        await cmd.ExecuteNonQueryAsync();
    }
    public async void DeleteProductFromShoppingcarts(){}
}