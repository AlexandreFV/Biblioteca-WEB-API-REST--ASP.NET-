using DTOS.Categoria;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TesteApiWeb.Class;
using TesteApiWeb.Context;
using TesteApiWeb.Models;
using static DTOS.Categoria.CategoriaDTO;

namespace TesteApiWeb.Services
{
    public class CategoriaService : ServicePersonalizado<Categoria>
    {
        private readonly AppDBContextSistema _context;

        public CategoriaService(AppDBContextSistema context)
        {
            _context = context;
        }

        public async Task<ServiceResult<IEnumerable<CategoriaResponseDTO>>> ListarCategoriasAsync()
        {
            var categorias = await _context.Categorias
                .AsNoTracking()
                .Select(c => new CategoriaResponseDTO
                {
                    CategoriaId = c.CategoriaId,
                    Nome = c.Nome,
                })
                .ToListAsync();

            if (!categorias.Any())
                return Result<IEnumerable<CategoriaResponseDTO>>(false, NaoEncontrado, null, ResultType.NotFound);

            return Result<IEnumerable<CategoriaResponseDTO>>(true, EncontradasSucesso, categorias, ResultType.Sucesso);
        }

        public async Task<ServiceResult<CategoriaResponseDTO>> CriarCategoriaAsync(CategoriaCreateDTO categoriaDto)
        {
            var nomeFormatado = PadronizarNome(categoriaDto.Nome);

            var categoriaExiste = await _context.Categorias.AsNoTracking().AnyAsync(c => c.Nome == nomeFormatado);

            if (categoriaExiste)
                return Result<CategoriaResponseDTO>(false, JaExisteEsseNome, null, ResultType.Conflito);

            var novaCategoria = new Categoria { Nome = nomeFormatado };

            try
            {
                _context.Categorias.Add(novaCategoria);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return Result<CategoriaResponseDTO>(
                    false,
                    ErroAoSalvar + "Db erro: " + ex,
                    null,
                    ResultType.Erro
                );
            }

            var resultadoDto = new CategoriaResponseDTO
            {
                CategoriaId = novaCategoria.CategoriaId,
                Nome = nomeFormatado,
            };

            return Result(true, AdicionadoSucesso, resultadoDto, ResultType.Criado);
        }

        public async Task<ServiceResult<CategoriaResponseDTO>> EditarCategoriaAsync(int id, CategoriaUpdateDTO categoriaDTO)
        {
            var categoriaBanco = await _context.Categorias.FindAsync(id);

            if (categoriaBanco == null)
                return Result<CategoriaResponseDTO>(false, NaoEncontrado, null, ResultType.NotFound);

            var nomeFormatado = PadronizarNome(categoriaDTO.Nome);

            var categoriaExiste = await _context.Categorias
                .AsNoTracking()
                .AnyAsync(c => c.Nome == nomeFormatado && c.CategoriaId != id);

            if (categoriaExiste)
                return Result<CategoriaResponseDTO>(false, JaExisteEsseNome, null, ResultType.Conflito);

            try
            {
                categoriaBanco.Nome = nomeFormatado;
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return Result<CategoriaResponseDTO>(
                    false,
                    ErroAoEditar + "Db erro: " + ex,
                    null,
                    ResultType.Erro
                );
            }

            var resultadoDto = new CategoriaResponseDTO
            {
                CategoriaId = id,
                Nome = nomeFormatado,
            };

            return Result(true, AtualizadoSucesso, resultadoDto, ResultType.Sucesso);
        }

        public async Task<ServiceResult<CategoriaResponseDTO>> ProcurarCategoriaAsync(int id)
        {
            var categoria = await _context.Categorias.AsNoTracking().FirstOrDefaultAsync(c => c.CategoriaId == id);

            if (categoria == null)
                return Result<CategoriaResponseDTO>(false, NaoEncontrado, null, ResultType.NotFound);

            var resultadoDto = new CategoriaResponseDTO
            {
                CategoriaId = id,
                Nome = categoria.Nome,
            };

            return Result(true, EncontradasSucesso, resultadoDto, ResultType.Sucesso);
        }


        public async Task<ServiceResult<bool>> ApagarCategoriaAsync(int id)
        {
            var categoriaBanco = await _context.Categorias.Include(c => c.Livros).FirstOrDefaultAsync(c => c.CategoriaId == id);

            if (categoriaBanco == null)
                return Result<bool>(false, NaoEncontrado, false, ResultType.NotFound);

            if (categoriaBanco.Livros != null && categoriaBanco.Livros.Any())
                return Result<bool>(false, RegistrosVinculados, false, ResultType.Conflito);


            try
            {
                _context.Categorias.Remove(categoriaBanco);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return Result<bool>(
                    false,
                    ErroAoApagar + "Db erro: " + ex,
                    false,
                    ResultType.Erro
                );
            }

            return Result<bool>(true, ExcluidoSucesso, true, ResultType.Sucesso);


        }
    }
}