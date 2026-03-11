using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Sistema.Application.Commoms.Bases;
using Sistema.Application.Interfaces;
using Sistema.Application.Interfaces.Services;
using Sistema.Application.Services;
using TesteApiWeb.Services;
using static DTOS.Auth.AuthDTO;
using static DTOS.Categoria.CategoriaDTO;
using static DTOS.Usuario.UsuarioDTO;

namespace Biblioteca_WEB_API_REST_ASP.Services
{
    public class AuthService : PadronizarTextoService,IAuthService
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IIdentityService identityService, ILogger<AuthService> logger)
        {
            _identityService = identityService;
            _logger = logger;
        }

        public async Task<ServiceResult<RegisterDTOResponse>> CriarUsuarioAsync(RegisterDTOCreate registerDTO, bool isAdmin)
        {
            _logger.LogInformation("Iniciando o service de criação do usuário");

            var emailPadronizado = PadronizarTexto(registerDTO.Email);
            var usuarioJáExiste = await _identityService.encontrarUsuarioPorEmailAsync(emailPadronizado);

            if (usuarioJáExiste != null)
            {
                _logger.LogWarning("Conflito ao tentar adicionar usuario com e-mail {Email}", emailPadronizado);

                return ServiceResult<RegisterDTOResponse>.Error(
                    null,
                    "Já existe um usuario com esse e-mail: " + registerDTO.Email,
                    ResultType.Conflito
                );

            }

            var usuario = new Usuario
            {
                Nome = PadronizarTexto(registerDTO.Nome),
                Email = (emailPadronizado),
                Ativo = true,
                UserName = (emailPadronizado),
            };

            var result = await _identityService.criarAsync(usuario, registerDTO.Senha);

            if (!result.Success)
            {
                _logger.LogError("Não foi possivel criar um usuario com e-mail: {Email}", emailPadronizado);

                return ServiceResult<RegisterDTOResponse>.Error(
                    null,
                    "Não foi possivel criar o usuario: " + string.Join(", ", result.Errors),
                    ResultType.Erro
                );
            }

            var roleResult = await _identityService.adicionarRoleAsync(usuario, isAdmin == true ? "Admin" : "Client");
            
            if (!roleResult)
            {
                _logger.LogError("Não foi possivel adicionar uma role ao usuario: {Email}", emailPadronizado);

                return ServiceResult<RegisterDTOResponse>.Error(
                        null,
                        "Não foi possivel definir a role para o usuario " + string.Join(", ", result.Errors),
                        ResultType.Erro
                    );
            }

            var usuarioExibir = new RegisterDTOResponse
            {
                Nome = usuario.Nome,
                Ativo = usuario.Ativo,
                UsuarioId = usuario.Id,
            };

            _logger.LogInformation("Usuario {Email} foi criado com sucesso no service", emailPadronizado);
            return ServiceResult<RegisterDTOResponse>.Success(
                usuarioExibir,
                "Usuario criado com sucesso",
                ResultType.Sucesso
            );
        }

        public async Task<ServiceResult<LoginResponseDTO>> EntrarAsync(LoginDTO loginDTO)
        {
            _logger.LogInformation("Iniciando o service de autenticação do usuario");

            var usuario = await _identityService.encontrarUsuarioPorEmailAsync(PadronizarTexto(loginDTO.Email));

            if (usuario == null)
            {
                _logger.LogWarning("Usuario {Email} não foi encontrado", loginDTO.Email);

                return ServiceResult<LoginResponseDTO>.Error(
                    null,
                    "Credenciais inválidas",
                    ResultType.NaoAutorizado
                );
            }

            if (!usuario.Ativo)
            {
                _logger.LogWarning("Usuario {Email} não se encontra ativo", loginDTO.Email);

                return ServiceResult<LoginResponseDTO>.Error(
                    null,
                    "Usuario inativo",
                    ResultType.NaoAutorizado
                );
            }

            var senhaValida = await _identityService.confirmarSenhaAsync(usuario, loginDTO.Senha);

            if (!senhaValida)
            {
                _logger.LogWarning("Tentativa de login com senha inválida");

                return ServiceResult<LoginResponseDTO>.Error(
                    null,
                    "Senha inválidos: ",
                    ResultType.NaoAutorizado
                );

            }

            var token = await _identityService.obterToken(usuario);

            _logger.LogInformation("Obtido Token do usuario {Email} com sucesso", loginDTO.Email);

            var usuarioLogado = new LoginResponseDTO
            {
                Token = token,
                UsuarioDTO = new UsuarioDTOLogin
                {
                    UsuarioId = usuario.Id,
                    Nome = usuario.Nome,
                    Ativo = usuario.Ativo,

                }
            };

            _logger.LogInformation("Autenticação do usuario {Email} com sucesso", loginDTO.Email);

            return ServiceResult<LoginResponseDTO>.Success(
                usuarioLogado,
                "Usuario logado com sucesso",
                ResultType.Sucesso
            );

        }
    }


}
