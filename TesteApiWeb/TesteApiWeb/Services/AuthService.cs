using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Models;
using Microsoft.AspNetCore.Identity;
using TesteApiWeb.Services;
using static DTOS.Auth.AuthDTO;
using static DTOS.Usuario.UsuarioDTO;

namespace Biblioteca_WEB_API_REST_ASP.Services
{
    public class AuthService : ServicePersonalizado<Usuario>
{

        private readonly UserManager<Usuario> _userManager;
        private readonly TokenService _tokenService;

        public AuthService(UserManager<Usuario> userManager, TokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<ServiceResult<RegisterDTOResponse>> CriarUsuarioAsync(RegisterDTOCreate registerDTO, bool isAdmin)
        {
            var usuarioJáExiste = await _userManager.FindByEmailAsync(registerDTO.Email);

            if (usuarioJáExiste != null)
                return Result<RegisterDTOResponse>(false, JaExisteEsseEmail, null, ResultType.Conflito);

            var usuario = new Usuario
            {
                Nome = PadronizarNome(registerDTO.Nome),
                Email = registerDTO.Email,
                Ativo = true,
                UserName = PadronizarNome(registerDTO.Nome),
            };

            var result = await _userManager.CreateAsync(usuario, registerDTO.Senha);

            if (!result.Succeeded)
            {
                return Result<RegisterDTOResponse>(
                    false,
                    string.Join(", ", result.Errors.Select(e => e.Description)),
                    null,
                    ResultType.Invalido
                );
            }

            var roleResult = await _userManager.AddToRoleAsync(usuario, isAdmin == true ? "Admin" : "Client");
            
            if (!roleResult.Succeeded)
                return Result<RegisterDTOResponse>(false, "Erro ao definir perfil", null, ResultType.Erro);

            var usuarioExibir = new RegisterDTOResponse
            {
                Nome = usuario.Nome,
                Ativo = usuario.Ativo,
                UsuarioId = usuario.Id,
            };

            return Result<RegisterDTOResponse>(true, AdicionadoSucesso, usuarioExibir, ResultType.Criado);
        }

        public async Task<ServiceResult<LoginResponseDTO>> LoginAsync(LoginDTO loginDTO)
        {
            var usuario = await _userManager.FindByEmailAsync(loginDTO.Email);

            if (usuario == null || !usuario.Ativo)
                return Result<LoginResponseDTO>(false, UsuarioOuSenhaInvalidos, null, ResultType.NaoAutorizado);

            var senhaValida = await _userManager.CheckPasswordAsync(usuario, loginDTO.Senha);

            if (!senhaValida)
                return Result<LoginResponseDTO>(false, UsuarioOuSenhaInvalidos, null, ResultType.NaoAutorizado);

            var token = await _tokenService.GerarToken(usuario);

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

            return Result(true, "Login realizado com sucesso", usuarioLogado, ResultType.Sucesso);
        }

    }


}
