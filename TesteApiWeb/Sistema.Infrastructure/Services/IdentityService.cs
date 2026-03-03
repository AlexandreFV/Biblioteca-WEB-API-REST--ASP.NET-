using Biblioteca_WEB_API_REST_ASP.Models;
using Microsoft.AspNetCore.Identity;
using Sistema.Application.Interfaces.Services;
using TesteApiWeb.Services;

namespace Sistema.Infrastructure.Services
{
    internal class IdentityService : IIdentityService
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly TokenService _tokenService;

        public IdentityService(UserManager<Usuario> userManager, TokenService tokenService) { _userManager = userManager; _tokenService = tokenService; }

        public async Task<bool> adicionarRoleAsync(Usuario user, string role)
        {
            var roleInserida = await _userManager.AddToRoleAsync(user, role);

            if (!roleInserida.Succeeded)
                return false;

            return true;
        }

        public async Task<bool> confirmarSenhaAsync(Usuario user, string senha)
        {
            return await _userManager.CheckPasswordAsync(user, senha);
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> criarAsync(Usuario user, string senha)
        {
            var novoUser = await _userManager.CreateAsync(user, senha);

            if (!novoUser.Succeeded)
                return (false, novoUser.Errors.Select(e => e.Description));

            return (true, Enumerable.Empty<string>());

        }

        public async Task<Usuario?> encontrarUsuarioPorEmailAsync(string email)
        {
            var userProcurado = await _userManager.FindByEmailAsync(email);

            if (userProcurado == null)
                return null;

            return userProcurado;
        }

        public async Task<Usuario?> encontrarUsuarioPorId(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return null;

            return user;
        }

        public async Task<string> obterToken(Usuario user)
        {
            return await _tokenService.GerarToken(user);

        }
    }
}
