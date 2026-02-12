using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace ReminderTelegramAPI.Configs;

public class DapperContext
{
    private readonly string _sqlServerConn;
    private readonly string _postgresConn;

    public DapperContext(IConfiguration config)
    {
        _sqlServerConn = config.GetConnectionString("SqlServerConnection")
            ?? throw new ArgumentNullException("SqlServerConnection");
        _postgresConn = config.GetConnectionString("PostgresConnection")
            ?? throw new ArgumentNullException("PostgresConnection");
    }

    public IDbConnection CreateSqlConnection()
    {
        var c = new SqlConnection(_sqlServerConn);
        c.Open();
        return c;
    }

    public IDbConnection CreatePostgresConnection()
    {
        var c = new NpgsqlConnection(_postgresConn);
        c.Open();
        return c;
    }
}
