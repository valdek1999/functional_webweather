using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using WebWeather.Setup;

namespace WebWeather.DataAccess
{
    /// <summary>
    /// Класс с фабричный методом, вызывающийся при создании миграций
    /// </summary>
    public class WeatherContextFactory : IDesignTimeDbContextFactory<DataWeatherContext>
    {
        public DataWeatherContext CreateDbContext(string[] args)
        {
            var settings = new Settings();
            settings.SetFromEnvironmentVariables();

            var optionsBuilder = new DbContextOptionsBuilder<DataWeatherContext>();
            optionsBuilder.UseNpgsql(settings.ConnectionStrings.DataWeatherContext);

            return new DataWeatherContext(optionsBuilder.Options, true);
        }
    }
}
