using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace SegurosApp.Infrastructure.Data
{
    public class SegurosDbContextFactory : IDesignTimeDbContextFactory<SegurosDbContext>
    {
        public SegurosDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory();
            string configFile = "appsettings.json";
            string configPath = Path.Combine(basePath, configFile);
            int maxDepth = 5;
            int depth = 0;
            while (!File.Exists(configPath) && depth < maxDepth)
            {
                basePath = Directory.GetParent(basePath)?.FullName ?? basePath;
                configPath = Path.Combine(basePath, configFile);
                depth++;
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile(configFile)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<SegurosDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            return new SegurosDbContext(optionsBuilder.Options);
        }
    }
}
