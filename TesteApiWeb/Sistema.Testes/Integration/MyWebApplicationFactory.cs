using Biblioteca_WEB_API_REST_ASP.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sistema.Testes.Integration
{
    public class MyWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["UseInMemory"] = "true",

                    // 🔥 JWT obrigatório
                    ["Jwt:Secret"] = "test_secret_key_123456789_123456789",
                    ["Jwt:Issuer"] = "Test",
                    ["Jwt:Audience"] = "Test"
                });
            });

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDBContextSistema>));

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<AppDBContextSistema>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // garantir roles criadas no teste
                var sp = services.BuildServiceProvider();

                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDBContextSistema>();
                db.Database.EnsureCreated();
            });
        }
    }
}