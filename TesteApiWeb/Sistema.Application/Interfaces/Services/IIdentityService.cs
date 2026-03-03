using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DTOS.Auth.AuthDTO;

namespace Sistema.Application.Interfaces.Services
{
    public interface IIdentityService
    {

        Task<Usuario?> encontrarUsuarioPorEmailAsync(string email);
        Task<bool> confirmarSenhaAsync(Usuario user, string senha);
        Task<(bool Success, IEnumerable<string> Errors)> criarAsync(Usuario user, string senha);
        Task<bool> adicionarRoleAsync(Usuario user, string role);
        Task<string> obterToken(Usuario user);
        Task<Usuario?> encontrarUsuarioPorId(string userId);
    }
}
