using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebWeather.DataAccess.Models;

namespace WebWeather.DataAccess
{
    public class DataWeatherContext: DbContext
    {
        public DbSet<Weather> Weather { get; set; } = null!;
        public DataWeatherContext(DbContextOptions<DataWeatherContext> options, bool isCreatingOfMigration = false)
            : base(options)
        {
            if (!isCreatingOfMigration)
            {
                Database.MigrateAsync().Wait();
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Weather>();
        }
    }
}
