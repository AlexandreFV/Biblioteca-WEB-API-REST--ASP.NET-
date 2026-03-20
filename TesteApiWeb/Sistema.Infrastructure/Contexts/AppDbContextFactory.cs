using Biblioteca_WEB_API_REST_ASP.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Biblioteca_WEB_API_REST_ASP
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDBContextSistema>
    {
        public AppDBContextSistema CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDBContextSistema>();

            // Pega a connection string da variável de ambiente
            var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");

            optionsBuilder.UseNpgsql(connectionString);

            return new AppDBContextSistema(optionsBuilder.Options);
        }
    }
}