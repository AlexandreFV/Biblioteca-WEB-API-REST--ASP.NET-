using Biblioteca_WEB_API_REST_ASP.Services;
using Microsoft.Extensions.DependencyInjection;
using Sistema.Application.Interfaces;
using Sistema.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteApiWeb.Services;

namespace Sistema.Application.Configurations
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Interface -> implementação
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<UsuarioService>();
            services.AddScoped<ILivroService, LivroService>();
            services.AddScoped<IAuthService, AuthService>();

            // services.AddScoped<ISolicitacaoEmprestimoService, SolicitacaoEmprestimoService>();

            return services;
        }
    }
}