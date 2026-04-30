using Biblioteca_WEB_API_REST_ASP.Context;
using Biblioteca_WEB_API_REST_ASP.Models;
using BibliotecaWebApiRest.Repositories.Concretas;
using BibliotecaWebApiRest.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Sistema.Application.Interfaces;
using Sistema.Application.Interfaces.Services;
using Sistema.Infrastructure.Services;
using System.Text;
using TesteApiWeb.Services;

namespace Sistema.Infrastructure.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration config,
            IHostEnvironment env)
        {

            if (env.IsEnvironment("Testing"))
            {
                services.AddDbContext<AppDBContextSistema>(options =>
                    options.UseInMemoryDatabase("TestDb"));
            }
            else
            {
                var connectionString = config.GetConnectionString("DefaultConnection");

                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new Exception("ConnectionString não configurada");

                services.AddDbContext<AppDBContextSistema>(options =>
                    options.UseNpgsql(connectionString));
            }

            services
                .AddIdentity<Usuario, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;
                })
                .AddEntityFrameworkStores<AppDBContextSistema>()
                .AddDefaultTokenProviders();

            // 🔥 JWT SEM EnvironmentVariable
            var jwtSecret =
                Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? config["Jwt:Secret"]
                ?? (env.IsEnvironment("Testing")
                    ? "test_secret_key_123456789_123456789"
                    : null);

            if (string.IsNullOrWhiteSpace(jwtSecret))
                throw new Exception("JWT Secret não configurado");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddHttpContextAccessor();

            services.AddScoped<TokenService>();
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ICurrentUser, CurrentUserService>();

            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<ILivroRepository, LivroRepository>();
            services.AddScoped<ISolicitacaoEmprestimoRepository, SolicitacaoEmprestimoRepository>();

            return services;
        }
    }
}