using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace erp_system.Configuration
{
    public static class AppConfig
    {
        private static readonly Lazy<IConfigurationRoot> _configuration = new Lazy<IConfigurationRoot>(BuildConfiguration);

        private static IConfigurationRoot BuildConfiguration()
        {
            var basePath = AppContext.BaseDirectory;

            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);

            return builder.Build();
        }

        public static string GetConnectionString(string name = "DefaultConnection")
        {
            var cs = _configuration.Value.GetConnectionString(name);
            return string.IsNullOrWhiteSpace(cs) ? string.Empty : cs;
        }
    }
}


