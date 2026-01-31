using DTOS.Categoria;
using DTOS.Livro;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using TesteApiWeb.Class;
using TesteApiWeb.Context;
using TesteApiWeb.Models;
using static DTOS.Categoria.CategoriaDTO;
using static DTOS.Livro.LivroDTO;

namespace TesteApiWeb.Services
{
    public class LivroService : ServicePersonalizado<Livro>
    {

        private readonly AppDBContextSistema _context;

        public LivroService(AppDBContextSistema context) { _context = context; }

        public async Task<ServiceResult<IEnumerable<LivroResponseDTO>>> ListarLivrosAsync()
        {
            var todosLivros = await _context.Livros.Include(l => l.Categorias).AsNoTracking().
                Select(l => new LivroResponseDTO
                {
                    LivroId = l.LivroId,
                    Nome = l.Nome,
                    Quantidade = l.Quantidade,
                    Categorias = l.Categorias!.Select(c => new CategoriaResponseDTO
                    {
                        CategoriaId = c.CategoriaId,
                        Nome = c.Nome,
                    }).ToList()
                })
                .ToListAsync();

            if (!todosLivros.Any())
                return Result<IEnumerable<LivroResponseDTO>>(false, NaoEncontrado, null, ResultType.NotFound);

            return Result<IEnumerable<LivroResponseDTO>>(true, EncontradasSucesso, todosLivros, ResultType.Sucesso);

        }

        public async Task<ServiceResult<LivroResponseDTO>> CriarLivroAsync(LivroCreateDTO livroDTO)
        {
            var nomeFormatado = PadronizarNome(livroDTO.Nome);

            if (await _context.Livros.AsNoTracking().AnyAsync(l => l.Nome == nomeFormatado))
                return Result<LivroResponseDTO>(false, JaExisteEsseNome, null, ResultType.Conflito);

            var idsCategorias = livroDTO.CategoriasId ?? new List<int>();

            var categoriasSelecionadas = await _context.Categorias
                .Where(c => idsCategorias.Contains(c.CategoriaId))
                .ToListAsync();

            if (idsCategorias.Count != categoriasSelecionadas.Count)
                return Result<LivroResponseDTO>(false, "Uma ou mais categorias informadas não foram encontradas.", null, ResultType.NotFound);

            var novoLivroEntity = new Livro
            {
                Nome = nomeFormatado,
                Quantidade = livroDTO.Quantidade,
                Categorias = categoriasSelecionadas,
            };


            try
            {
                _context.Livros.Add(novoLivroEntity);
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateException ex)
            {
                return Result<LivroResponseDTO>(
                    false,
                    ErroAoSalvar + "Db Erro: " + ex,
                    null,
                    ResultType.Erro
                );

            }

            var result = new LivroResponseDTO
            {
                LivroId = novoLivroEntity.LivroId,
                Nome = novoLivroEntity.Nome,
                Quantidade = novoLivroEntity.Quantidade,
                Categorias = categoriasSelecionadas.Select(c => new CategoriaResponseDTO
                {
                    CategoriaId = c.CategoriaId,
                    Nome = c.Nome
                }).ToList()
            };

            return Result<LivroResponseDTO>(true, AdicionadoSucesso, result, ResultType.Criado);

        }

        public async Task<ServiceResult<LivroResponseDTO>> EditarLivroAsync(int id, LivroEditDTO livroDTO)
        {
            var livroProcurado = await _context.Livros.Include(l => l.Categorias).FirstOrDefaultAsync(l => l.LivroId == id);

            if (livroProcurado == null)
                return Result<LivroResponseDTO>(false, NaoEncontrado, null, ResultType.NotFound);

            livroProcurado.Nome = PadronizarNome(livroDTO.Nome);
            livroProcurado.Quantidade = livroDTO.Quantidade;

            var ids = livroDTO.CategoriasId ?? new List<int>();
            var novasCategorias = await _context.Categorias
                .Where(c => ids.Contains(c.CategoriaId))
                .ToListAsync();

            try
            {
                livroProcurado.Categorias = novasCategorias;
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateException ex)
            {
                return Result<LivroResponseDTO>(
                    false,
                    ErroAoEditar + "Db Erro: " + ex,
                    null,
                    ResultType.Erro
                );
            }

            var result = new LivroResponseDTO
            {
                LivroId = livroProcurado.LivroId,
                Nome = livroProcurado.Nome,
                Quantidade = livroProcurado.Quantidade,
                Categorias = livroProcurado.Categorias!.Select(c => new CategoriaResponseDTO
                {
                    CategoriaId = c.CategoriaId,
                    Nome = c.Nome,
                }).ToList()
            };

            return Result(true, AtualizadoSucesso, result, ResultType.Sucesso);

        }

        public async Task<ServiceResult<LivroResponseDTO>> ProcurarLivroPorIdAsync(int id)
        {
            var livroBanco = await _context.Livros
                    .Include(l => l.Categorias)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => l.LivroId == id);

            if (livroBanco == null)
                return Result<LivroResponseDTO>(false, NaoEncontrado, null, ResultType.NotFound);

            var livroDTO = new LivroResponseDTO
            {
                Nome = livroBanco.Nome,
                Quantidade = livroBanco.Quantidade,
                LivroId = livroBanco.LivroId,
                Categorias = livroBanco.Categorias!.Select(c => new CategoriaResponseDTO
                {
                    Nome = c.Nome,
                    CategoriaId = c.CategoriaId,
                }).ToList(),
            };

            return Result<LivroResponseDTO>(true, EncontradasSucesso, livroDTO, ResultType.Sucesso);

        }

        public async Task<ServiceResult<bool>> ApagarLivroAsync(int id)
        {
            var livroProcurado = await _context.Livros.Include(l => l.Categorias).FirstOrDefaultAsync(l => l.LivroId == id);

            if (livroProcurado == null)
                return Result<bool>(false, NaoEncontrado, false, ResultType.NotFound);

            if (livroProcurado.Categorias != null && livroProcurado.Categorias.Any())
                return Result<bool>(false, RegistrosVinculados, false, ResultType.Conflito);

            try
            {
                _context.Livros.Remove(livroProcurado);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return Result<bool>(
                    false,
                    ErroAoApagar  + "Db Erro: " + ex,
                    false,
                    ResultType.Erro
                );
            }

            return Result<bool>(true, ExcluidoSucesso, true, ResultType.Sucesso);
        }
    }
}
