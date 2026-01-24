using Microsoft.EntityFrameworkCore;
using TesteApiWeb.Class;
using TesteApiWeb.Context;
using TesteApiWeb.Models;

namespace TesteApiWeb.Services
{
    public class UsuarioService : ServicePersonalizado<Usuario>
    {

        private readonly AppDBContextSistema _context;

        public UsuarioService(AppDBContextSistema context){_context = context;}


        public ServiceResult<IEnumerable<UsuarioDTO>> ListarUsuarios()
        {
            var usuarios = _context.Usuarios.AsNoTracking()
                .Select(u => new UsuarioDTO
                {
                    UsuarioId = u.UsuarioId,
                    Nome = u.Nome,
                    Senha = u.Senha, /*TODO FUTURAMENTE TEM QUE CONVERTER A SENHA PARA UM HASH OU ALGO DO GENERO*/
                    Tipo = u.Tipo,
                }).ToList();

            if(!usuarios.Any())
                return Result<IEnumerable<UsuarioDTO>>(false, NaoEncontrado, null, ResultType.NotFound);

            return Result<IEnumerable<UsuarioDTO>>(true, EncontradasSucesso, usuarios, ResultType.Sucesso);

        }

        public ServiceResult<UsuarioDTO> CriarUsuario(UsuarioDTO usuarioDTO)
        {
            var nomeFormatado = PadronizarNome(usuarioDTO.Nome);

            var tipoFormatado = PadronizarNome(usuarioDTO.Tipo);

            var usuarioEntity = new Usuario { Nome = usuarioDTO.Nome, Tipo = usuarioDTO.Tipo, Senha = usuarioDTO.Senha };

            _context.Usuarios.Add(usuarioEntity);
            _context.SaveChanges();

            var usuarioDTOBanco = EntityToDTO(usuarioEntity);

            return Result(true, AdicionadoSucesso, usuarioDTOBanco, ResultType.Criado);
        }


        public ServiceResult<UsuarioDTO> EditarUsuario(int id, UsuarioDTO usuarioDTO)
        {
            var usuarioBancoProcurar = _context.Usuarios.Find(id);

            if (usuarioBancoProcurar == null)
                return Result<UsuarioDTO>(false, NaoEncontrado, null, ResultType.NotFound);

            if (usuarioDTO.UsuarioId != id)
                return Result<UsuarioDTO>(false, IdDiferente, null, ResultType.Invalido);

            usuarioBancoProcurar.Nome = PadronizarNome(usuarioDTO.Nome);
            usuarioBancoProcurar.Tipo = PadronizarNome(usuarioDTO.Tipo);
            usuarioBancoProcurar.Senha = usuarioDTO.Senha;
            _context.SaveChanges();

            var novoUsuarioDTO = EntityToDTO(usuarioBancoProcurar);

            return Result(true, AtualizadoSucesso, novoUsuarioDTO, ResultType.Atualizado);
        }

        public ServiceResult<UsuarioDTO> ProcurarUsuario(int id)
        {
            var usuarioBancoProcurar = _context.Usuarios.AsNoTracking().FirstOrDefault(u => u.UsuarioId == id);

            if (usuarioBancoProcurar == null)
                return Result<UsuarioDTO>(false, NaoEncontrado, null, ResultType.NotFound);

            var novoUsuarioDTO = EntityToDTO(usuarioBancoProcurar);

            return Result(true, EncontradasSucesso, novoUsuarioDTO, ResultType.Sucesso);

        }

        public ServiceResult<bool> ApagarUsuario(int id)
        {

            var usuarioBancoProcurar = _context.Usuarios.AsNoTracking().FirstOrDefault(u => u.UsuarioId == id);

            if (usuarioBancoProcurar == null)
                return Result<bool>(false, NaoEncontrado, false, ResultType.NotFound);

            /*TODO
             CASO USUARIO TIVER UM LIVRO VINCULADO ELE N PODE SER EXCLUIDO*/

            _context.Usuarios.Remove(usuarioBancoProcurar);
            _context.SaveChanges();

            return Result<bool>(true, ExcluidoSucesso, true, ResultType.Sucesso);

        }
        private UsuarioDTO EntityToDTO(Usuario usuarioEntity)
        {
            return new UsuarioDTO
            {
                UsuarioId = usuarioEntity.UsuarioId,
                Nome = usuarioEntity.Nome,
                Tipo = usuarioEntity.Tipo,
                Senha = usuarioEntity.Senha,
            };
        }
    }
}
