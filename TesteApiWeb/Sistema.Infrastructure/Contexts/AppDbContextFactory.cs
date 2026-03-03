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

            // Mesma string que você coloca no appsettings.json
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=SistemaApiDotNet;Trusted_Connection=True;TrustServerCertificate=True;");

            return new AppDBContextSistema(optionsBuilder.Options);
        }
    }
}
