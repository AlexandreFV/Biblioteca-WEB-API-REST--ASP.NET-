using DTOS.Usuario;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata.Ecma335;
using TesteApiWeb.Class;
using TesteApiWeb.Models;
using static DTOS.Categoria.CategoriaDTO;
using static DTOS.Usuario.UsuarioDTO;
using static TesteApiWeb.Models.AuthDTO;

namespace TesteApiWeb.Services
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

        public async Task<ServiceResult<RegisterDTOResponse>> CriarUsuarioAsync(RegisterDTOCreate registerDTO)
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

            var usuarioEntity = new Usuario
            {
                Id = usuario.Id,
                Ativo = usuario.Ativo,
                Email = usuario.Email,
                Nome = usuario.Nome,
                UserName = usuario.UserName,
            };

            var token = _tokenService.GerarToken(usuarioEntity);

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
