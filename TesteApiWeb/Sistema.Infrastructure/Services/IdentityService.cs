using Biblioteca_WEB_API_REST_ASP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Sistema.Application.Interfaces.Services;
using TesteApiWeb.Services;

namespace Sistema.Infrastructure.Services
{
    internal class IdentityService : IIdentityService
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly TokenService _tokenService;
        private readonly ILogger<IdentityService> _logger;

        public IdentityService(
            UserManager<Usuario> userManager,
            TokenService tokenService,
            ILogger<IdentityService> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<bool> adicionarRoleAsync(Usuario user, string role)
        {
            _logger.LogInformation(
                "Iniciando inserção da role {Role} ao usuário {Usuario}",
                role,
                user.Nome);

            var roleInserida = await _userManager.AddToRoleAsync(user, role);

            if (!roleInserida.Succeeded)
            {
                foreach (var error in roleInserida.Errors)
                {
                    _logger.LogError(
                        "Erro ao adicionar role {Role} ao usuário {Usuario}: {Erro}",
                        role,
                        user.Nome,
                        error.Description);
                }

                return false;
            }

            _logger.LogInformation(
                "Role {Role} adicionada ao usuário {Usuario} com sucesso",
                role,
                user.Nome);

            return true;
        }

        public async Task<bool> confirmarSenhaAsync(Usuario user, string senha)
        {
            _logger.LogInformation(
                "Realizando verificação de senha do usuário {Usuario}",
                user.Nome);

            return await _userManager.CheckPasswordAsync(user, senha);
        }

        public async Task<(bool Success, IEnumerable<string> Errors)> criarAsync(Usuario user, string senha)
        {
            _logger.LogInformation(
                "Iniciando criação do usuário {Usuario} com Identity",
                user.Nome);

            var novoUser = await _userManager.CreateAsync(user, senha);

            if (!novoUser.Succeeded)
            {
                foreach (var error in novoUser.Errors)
                {
                    _logger.LogError(
                        "Erro ao criar usuário {Usuario}: {Erro}",
                        user.Nome,
                        error.Description);
                }

                return (false, novoUser.Errors.Select(e => e.Description));
            }

            _logger.LogInformation(
                "Usuário {Usuario} criado com sucesso",
                user.Nome);

            return (true, Enumerable.Empty<string>());
        }

        public async Task<Usuario?> encontrarUsuarioPorEmailAsync(string email)
        {
            _logger.LogInformation(
                "Iniciando busca do usuário com e-mail {Email}",
                email);

            var userProcurado = await _userManager.FindByEmailAsync(email);

            if (userProcurado == null)
            {
                _logger.LogWarning(
                    "Usuário com e-mail {Email} não encontrado",
                    email);

                return null;
            }

            _logger.LogInformation(
                "Usuário com e-mail {Email} encontrado com sucesso",
                email);

            return userProcurado;
        }

        public async Task<Usuario?> encontrarUsuarioPorId(string userId)
        {
            _logger.LogInformation(
                "Iniciando busca do usuário com id {UserId}",
                userId);

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning(
                    "Usuário com id {UserId} não encontrado",
                    userId);

                return null;
            }

            _logger.LogInformation(
                "Usuário com id {UserId} encontrado com sucesso",
                userId);

            return user;
        }

        public async Task<string> obterToken(Usuario user)
        {
            _logger.LogInformation(
                "Gerando token para o usuário {Usuario}",
                user.Email);

            return await _tokenService.GerarToken(user);
        }
    }
}