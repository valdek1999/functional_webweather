using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace WebWeather.Setup
{
    public static class SettingsExtensions
    {
        public static void SetFromEnvironmentVariables(this Settings settings)
        {
            
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            config.Bind(settings);
        }
    }
}