using Biblioteca_WEB_API_REST_ASP.Context;
using Biblioteca_WEB_API_REST_ASP.Models;
using BibliotecaWebApiRest.Repositories.Concretas;
using BibliotecaWebApiRest.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            IConfiguration config)
        {
            // 1️⃣ Configura DbContext
            var connectionString = config.GetConnectionString("SistemaAPIWeb");
            services.AddDbContext<AppDBContextSistema>(options =>
                options.UseSqlServer(connectionString));

            // 2️⃣ Configura Identity
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

            // 3️⃣ Configura JWT
            var jwtSecret = config["Jwt:Secret"]
                ?? throw new Exception("JWT Secret não configurado");

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

            // 4️⃣ Serviços técnicos (TokenService precisa ser registrado antes do IdentityService)
            services.AddScoped<TokenService>();

            // 5️⃣ Serviços de aplicação (IdentityService depende do TokenService)
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<ICurrentUser, CurrentUserService>();

            // 6️⃣ Repositórios
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();
            services.AddScoped<ILivroRepository, LivroRepository>();
            services.AddScoped<ISolicitacaoEmprestimoRepository, SolicitacaoEmprestimoRepository>();

            return services;
        }
    }
}
