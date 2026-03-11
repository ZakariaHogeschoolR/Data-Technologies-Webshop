using System.Data;
using Npgsql;

namespace ApplicationDbContext
{
    public class DatabaseConnectie
    {
        private readonly string _connectionString;
        public DatabaseConnectie(string connectionString)
        {
            _connectionString = connectionString;
        }

        public NpgsqlConnection GetConnection()
        {
            var conn = new NpgsqlConnection(_connectionString);
            conn.Open();
            return conn;

        }
    }
}