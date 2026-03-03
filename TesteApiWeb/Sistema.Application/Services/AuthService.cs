using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Models;
using Microsoft.AspNetCore.Identity;
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

        public AuthService(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<ServiceResult<RegisterDTOResponse>> CriarUsuarioAsync(RegisterDTOCreate registerDTO, bool isAdmin)
        {
            var emailPadronizado = PadronizarTexto(registerDTO.Email);
            var usuarioJáExiste = await _identityService.encontrarUsuarioPorEmailAsync(emailPadronizado);

            if (usuarioJáExiste != null)
                    return ServiceResult<RegisterDTOResponse>.Error(
                        null,
                        "Já existe um usuario com esse e-mail: " + registerDTO.Email,
                        ResultType.Conflito
                    );

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
                return ServiceResult<RegisterDTOResponse>.Error(
                    null,
                    "Não foi possivel criar o usuario: " + string.Join(", ", result.Errors),
                    ResultType.Erro
                );
            }

            var roleResult = await _identityService.adicionarRoleAsync(usuario, isAdmin == true ? "Admin" : "Client");
            
            if (!roleResult)
                return ServiceResult<RegisterDTOResponse>.Error(
                    null,
                    "Não foi possivel definir a role para o usuario " + string.Join(", ", result.Errors),
                    ResultType.Erro
                );

            var usuarioExibir = new RegisterDTOResponse
            {
                Nome = usuario.Nome,
                Ativo = usuario.Ativo,
                UsuarioId = usuario.Id,
            };

            return ServiceResult<RegisterDTOResponse>.Success(
                usuarioExibir,
                "Usuario criado com sucesso",
                ResultType.Sucesso
            );
        }

        public async Task<ServiceResult<LoginResponseDTO>> EntrarAsync(LoginDTO loginDTO)
        {
            var usuario = await _identityService.encontrarUsuarioPorEmailAsync(PadronizarTexto(loginDTO.Email));

            if (usuario == null || !usuario.Ativo)
                return ServiceResult<LoginResponseDTO>.Error(
                    null,
                    "Email erro: ",
                    ResultType.NaoAutorizado
                );

            var senhaValida = await _identityService.confirmarSenhaAsync(usuario, loginDTO.Senha);

            if (!senhaValida)
                return ServiceResult<LoginResponseDTO>.Error(
                    null,
                    "Senha inválidos: ",
                    ResultType.NaoAutorizado
                );

            var token = await _identityService.obterToken(usuario);

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

            return ServiceResult<LoginResponseDTO>.Success(
                usuarioLogado,
                "Usuario logado com sucesso",
                ResultType.Sucesso
            );

        }
    }


}
