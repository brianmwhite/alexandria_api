using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.Data.Sqlite;
using alexandria.api.Entities;

namespace alexandria.api.Helpers;
public class AppDataContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public AppDataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to sqlite database
        options.UseSqlite(Configuration.GetConnectionString("AppDatabase"));
    }

    public DbSet<DeviceType> DeviceTypes { get; set; }
    public DbSet<KnownDevice> KnownDevices { get; set; }
}
