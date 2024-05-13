namespace alexandria.api.Helpers;

using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;

public class BookDataContext
{
    protected readonly IConfiguration Configuration;

    public BookDataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IDbConnection CreateConnection()
    {
        return new SqliteConnection(Configuration.GetConnectionString("BookDatabase"));
    }
}