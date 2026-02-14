using DTOS.Usuario;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Context;
using Biblioteca_WEB_API_REST_ASP.Models;
using static DTOS.Usuario.UsuarioDTO;

namespace TesteApiWeb.Services
{
    public class UsuarioService : ServicePersonalizado<Usuario>
    {

        private readonly UserManager<Usuario> _userManager;

        public UsuarioService(UserManager<Usuario> userManager) {_userManager = userManager;}


        public async Task<ServiceResult<IEnumerable<UsuarioDTOResponse>>> ListarUsuariosAsync()
        {
            var usuarios = await _userManager.Users.AsNoTracking()
                .Select(u => new UsuarioDTOResponse
                {
                    UsuarioId = u.Id,
                    Nome = u.Nome,
                    Ativo = u.Ativo,
                }).ToListAsync();

            if(!usuarios.Any())
                return Result<IEnumerable<UsuarioDTOResponse>>(false, NaoEncontrado, null, ResultType.NotFound);

            return Result<IEnumerable<UsuarioDTOResponse>>(true, EncontradasSucesso, usuarios, ResultType.Sucesso);

        }

        public async Task<ServiceResult<UsuarioDTOResponse>> EditarUsuarioAsync(string id, UsuarioDTOEdit usuarioDTO)
        {
            var usuarioBancoProcurar = await _userManager.FindByIdAsync(id);

            if (usuarioBancoProcurar == null)
                return Result<UsuarioDTOResponse>(false, NaoEncontrado, null, ResultType.NotFound);

            usuarioBancoProcurar.Nome = PadronizarNome(usuarioDTO.Nome);
            usuarioBancoProcurar.Ativo = usuarioDTO.Ativo;
           
            var result = await _userManager.UpdateAsync(usuarioBancoProcurar);

            if (!result.Succeeded)
                return Result<UsuarioDTOResponse>(
                    false,
                    string.Join(", ", result.Errors.Select(e => e.Description)),
                    null,
                    ResultType.Invalido
                );

            var novoUsuarioExibir = new UsuarioDTOResponse
            {
                Nome = usuarioBancoProcurar.Nome,
                Ativo = true,
                UsuarioId = usuarioBancoProcurar.Id,
            };


            return Result<UsuarioDTOResponse>(true, AtualizadoSucesso, novoUsuarioExibir, ResultType.Atualizado);
        }

        public async Task<ServiceResult<UsuarioDTOResponse>> ProcurarUsuarioAsync(string id)
        {
            var usuarioBancoProcurar = await _userManager.FindByIdAsync(id);

            if (usuarioBancoProcurar == null)
                return Result<UsuarioDTOResponse>(false, NaoEncontrado, null, ResultType.NotFound);


            var novoUsuarioExibir = new UsuarioDTOResponse
            {
                Nome = usuarioBancoProcurar.Nome,
                Ativo = true,
                UsuarioId = usuarioBancoProcurar.Id,
            };

            return Result(true, EncontradasSucesso, novoUsuarioExibir, ResultType.Sucesso);

        }

        public async Task<ServiceResult<bool>> ApagarUsuarioAsync(string id)
        {

            var usuarioBancoProcurar = await _userManager.FindByIdAsync(id);

            if (usuarioBancoProcurar == null)
                return Result<bool>(false, NaoEncontrado, false, ResultType.NotFound);

            /*TODO
             CASO USUARIO TIVER UM EMPRESTIMO ATIVO VINCULADO ELE N PODE SER EXCLUIDO*/
               
            usuarioBancoProcurar.Ativo = false;

            var result = await _userManager.UpdateAsync(usuarioBancoProcurar);

            if (!result.Succeeded)
                return Result<bool>(
                    false,
                    string.Join(", ", result.Errors.Select(e => e.Description)),
                    false,
                    ResultType.Invalido
                );

            return Result<bool>(true, ExcluidoSucesso, true, ResultType.Sucesso);

        }
    }
}
