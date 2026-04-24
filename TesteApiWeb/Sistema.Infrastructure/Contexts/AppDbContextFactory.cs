using Biblioteca_WEB_API_REST_ASP.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Biblioteca_WEB_API_REST_ASP
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDBContextSistema>
    {
        public AppDBContextSistema CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets<AppDbContextFactory>() 
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new Exception("ConnectionString não encontrada.");

            var optionsBuilder = new DbContextOptionsBuilder<AppDBContextSistema>();
            optionsBuilder.UseNpgsql(connectionString);

            return new AppDBContextSistema(optionsBuilder.Options);
        }
    }
}