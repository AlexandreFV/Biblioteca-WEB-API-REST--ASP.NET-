using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Context;
using Biblioteca_WEB_API_REST_ASP.Models;
using static DTOS.Categoria.CategoriaDTO;
using static DTOS.Livro.LivroDTO;

namespace TesteApiWeb.Services
{
    public class LivroService : ServicePersonalizado<Livro>
    {

        private readonly AppDBContextSistema _context;

        public LivroService(AppDBContextSistema context) { _context = context; }

        public async Task<ServiceResult<IEnumerable<LivroResponseDTO>>> ListarLivrosAsync(ClaimsPrincipal userClaim)
        {

            var isAdmin = userClaim.IsInRole("Admin");


            var livros = await AplicarFiltroVisualizacao(_context.Livros.AsNoTracking(), isAdmin)
                .Select(l => new LivroResponseDTO
                {
                    LivroId = l.LivroId,
                    Nome = l.Nome,
                    Quantidade = l.Quantidade,
                    Categorias = l.Categorias!.Select(c => new CategoriaResponseDTO
                    {
                        CategoriaId = c.CategoriaId,
                        Nome = c.Nome
                    }).ToList()
                })
                .ToListAsync();

            return Result<IEnumerable<LivroResponseDTO>>(true, EncontradasSucesso, livros, ResultType.Sucesso);

        }

        public async Task<ServiceResult<LivroResponseDTO>> CriarLivroAsync(LivroCreateDTO livroDTO)
        {
            var nomeFormatado = PadronizarNome(livroDTO.Nome);

            var nomeJaExiste = await _context.Livros
                .AsNoTracking() 
                .AnyAsync(l => l.Nome == nomeFormatado && l.Ativo);

            if (nomeJaExiste)
                return Result<LivroResponseDTO>(false, JaExisteEsseNome, null, ResultType.Conflito);

            var categorias = await _context.Categorias
                .Where(c => livroDTO.CategoriasId!.Contains(c.CategoriaId) && c.Ativo)
                .ToListAsync();

            if (categorias.Count != livroDTO.CategoriasId!.Count)
                return Result<LivroResponseDTO>(false, "Categoria inválida.", null, ResultType.NotFound);

            var novoLivroEntity = new Livro
            {
                Nome = nomeFormatado,
                Quantidade = livroDTO.Quantidade,
                Categorias = categorias,
                Ativo = true
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

            return Result<LivroResponseDTO>(true, AdicionadoSucesso, Mapear(novoLivroEntity), ResultType.Criado);

        }

        public async Task<ServiceResult<LivroResponseDTO>> EditarLivroAsync(int id, LivroEditDTO livroDTO)
        {
            var livroProcurado = await _context.Livros.Include(l => l.Categorias).FirstOrDefaultAsync(l => l.LivroId == id);

            if (livroProcurado == null)
                return Result<LivroResponseDTO>(false, NaoEncontrado, null, ResultType.NotFound);

            var nomeFormatado = PadronizarNome(livroDTO.Nome);

            var nomeDuplicado = await _context.Livros
                .AnyAsync(l => l.LivroId != id && l.Nome == nomeFormatado && l.Ativo);

            if (nomeDuplicado)
                return Result<LivroResponseDTO>(false, JaExisteEsseNome, null, ResultType.Conflito);

            var categorias = await _context.Categorias
                .Where(c => livroDTO.CategoriasId!.Contains(c.CategoriaId) && c.Ativo)
                .ToListAsync();

            if (categorias.Count != livroDTO.CategoriasId!.Count)
                return Result<LivroResponseDTO>(false, "Categoria inválida.", null, ResultType.NotFound);

            livroProcurado.Nome = nomeFormatado;
            livroProcurado.Quantidade = livroDTO.Quantidade;
            livroProcurado.Categorias = categorias;

            try
            {
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

            return Result(true, AtualizadoSucesso, Mapear(livroProcurado), ResultType.Sucesso);

        }

        public async Task<ServiceResult<LivroResponseDTO>> ProcurarLivroPorIdAsync(int id, ClaimsPrincipal userClaim)
        {

            var isAdmin = userClaim.IsInRole("Admin");

            var livroBanco = await _context.Livros
                    .AsNoTracking()
                    .Include(l => l.Categorias)
                    .FirstOrDefaultAsync(l => l.LivroId == id);

            if (livroBanco == null || (!isAdmin && (!livroBanco.Ativo || livroBanco.Quantidade <= 0)))
                return Result<LivroResponseDTO>(false, NaoEncontrado, null, ResultType.NotFound);

            return Result(true, EncontradasSucesso, Mapear(livroBanco), ResultType.Sucesso);

        }

        public async Task<ServiceResult<bool>> ApagarLivroAsync(int id)
        {
            var livroProcurado = await _context.Livros.FirstOrDefaultAsync(l => l.LivroId == id);

            if (livroProcurado == null)
                return Result<bool>(false, NaoEncontrado, false, ResultType.NotFound);

            /*TODO Adicionar a verificação de emprestimo ativo*/
            livroProcurado.Ativo = false;

            try
            {
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

        private IQueryable<Livro> AplicarFiltroVisualizacao(IQueryable<Livro> query, bool isAdmin)
        {

            if (!isAdmin)
                query = query.Where(l => l.Quantidade > 0 && l.Ativo);

            return query;
        }

        private LivroResponseDTO Mapear(Livro l) 
        {
            var LivroMapeado = new LivroResponseDTO
            {
                LivroId = l.LivroId,
                Nome = l.Nome,
                Quantidade = l.Quantidade,
                Categorias = l.Categorias!.Select(c => new CategoriaResponseDTO
                {
                    CategoriaId = c.CategoriaId,
                    Nome = c.Nome
                }).ToList()
            };

            return LivroMapeado;
        }

    }
}
