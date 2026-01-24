using Microsoft.EntityFrameworkCore;
using TesteApiWeb.Class;
using TesteApiWeb.Context;
using TesteApiWeb.Models;

namespace TesteApiWeb.Services
{
    public class CategoriaService : ServicePersonalizado<Categoria>
    {
        private readonly AppDBContextSistema _context;

        public CategoriaService(AppDBContextSistema context)
        {
            _context = context;
        }

        public ServiceResult<IEnumerable<CategoriaDTO>> ListarCategorias()
        {
            var categorias = _context.Categorias
                .AsNoTracking()
                .Select(c => new CategoriaDTO
                {
                    CategoriaId = c.CategoriaId,
                    Nome = c.Nome,
                })
                .ToList();

            if (!categorias.Any())
                return Result<IEnumerable<CategoriaDTO>>(false, NaoEncontrado, null, ResultType.NotFound);

            return Result<IEnumerable<CategoriaDTO>>(true, EncontradasSucesso, categorias, ResultType.Sucesso);
        }

        public ServiceResult<CategoriaDTO> CriarCategoria(CategoriaDTO categoriaDto)
        {
            var nomeFormatado = PadronizarNome(categoriaDto.Nome);

            if (_context.Categorias.AsNoTracking().Any(c => c.Nome == nomeFormatado))
                return Result<CategoriaDTO>(false, JaExisteEsseNome, null, ResultType.Conflito);

            var novaCategoria = new Categoria { Nome = nomeFormatado };

            _context.Categorias.Add(novaCategoria);
            _context.SaveChanges();

            var resultadoDto = MapToDTO(novaCategoria);

            return Result(true, AdicionadoSucesso, resultadoDto, ResultType.Criado);
        }

        public ServiceResult<CategoriaDTO> EditarCategoria(int id, CategoriaDTO categoriaDTO)
        {
            var categoriaBanco = _context.Categorias.Find(id);
            if (categoriaBanco == null)
                return Result<CategoriaDTO>(false, NaoEncontrado, null, ResultType.NotFound);

            if (categoriaDTO.CategoriaId != id)
                return Result<CategoriaDTO>(false, IdDiferente, null, ResultType.Conflito);

            var nomeFormatado = PadronizarNome(categoriaDTO.Nome);

            if (_context.Categorias.Any(c => c.Nome == nomeFormatado && c.CategoriaId != id))
                return Result<CategoriaDTO>(false, JaExisteEsseNome, null, ResultType.Conflito);

            categoriaBanco.Nome = nomeFormatado;
            _context.SaveChanges();

            var resultadoDto = MapToDTO(categoriaBanco);

            return Result(true, AtualizadoSucesso, resultadoDto, ResultType.Sucesso);
        }

        public ServiceResult<CategoriaDTO> ProcurarCategoria(int id)
        {
            var categoria = _context.Categorias.AsNoTracking().FirstOrDefault(c => c.CategoriaId == id);

            if (categoria == null)
                return Result<CategoriaDTO>(false, NaoEncontrado, null, ResultType.NotFound);

            var resultadoDto = MapToDTO(categoria);

            return Result(true, EncontradasSucesso, resultadoDto, ResultType.Sucesso);
        }


        public ServiceResult<bool> ApagarCategoria(int id)
        {
            var categoriaBanco = _context.Categorias.Include(c => c.Livros).FirstOrDefault(c => c.CategoriaId == id);

            if (categoriaBanco == null)
                return Result<bool>(false, NaoEncontrado, false, ResultType.NotFound);

            if (categoriaBanco.Livros != null && categoriaBanco.Livros.Any())
                return Result<bool>(false, RegistrosVinculados, false, ResultType.Conflito);

            _context.Categorias.Remove(categoriaBanco);
            _context.SaveChanges();

            return Result<bool>(true, ExcluidoSucesso, true, ResultType.Sucesso);


        }
        private CategoriaDTO MapToDTO(Categoria categoria)
        {
            return new CategoriaDTO
            {
                CategoriaId = categoria.CategoriaId,
                Nome = categoria.Nome
            };
        }
    }
}