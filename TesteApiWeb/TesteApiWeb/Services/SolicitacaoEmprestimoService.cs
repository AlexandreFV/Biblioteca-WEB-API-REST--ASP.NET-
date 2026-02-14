using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Context;
using Biblioteca_WEB_API_REST_ASP.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static DTOS.Livro.LivroDTO;
using static DTOS.SolicitacaoEmprestimo.SolicitacaoEmprestimoDTO;
using static DTOS.Usuario.UsuarioDTO;

namespace Biblioteca_WEB_API_REST_ASP.Services
{
    public class SolicitacaoEmprestimoService : ServicePersonalizado<SolicitacaoEmprestimo>
{

        private readonly AppDBContextSistema _context;

        public SolicitacaoEmprestimoService (AppDBContextSistema context)
        {
            _context = context; 
        }

        public async Task<ServiceResult<IEnumerable<SolicitacaoEmprestimoDTOResponse>>> ListarSolicitacoesEmprestimosAsync(ClaimsPrincipal userClaim)
        {
            var isAdmin = ObterPermiss(userClaim);
            var userId = ObterIdUser(userClaim);

            var todosSolicitacoesEmprestimos = await AplicarFiltroVisualizacao(_context.Solicitacoes.AsNoTracking(), isAdmin, userId!)
                .Include(s => s.UsuarioSolicitante)
                .Include(s => s.LivroSolicitado)
                .Select(s => new SolicitacaoEmprestimoDTOResponse
                {
                    Id = s.Id,
                    IdUsuarioEmprestimo = s.IdUsuarioEmprestimo,
                    IdLivro = s.IdLivro,
                    DataSolicitacao = s.DataSolicitacao,
                    Status = s.Status,
                    UsuarioSolicitante = s.UsuarioSolicitante == null ? null : new UsuarioDTOResponse
                    {
                        UsuarioId = s.UsuarioSolicitante.Id,
                        Nome = s.UsuarioSolicitante.Nome,
                        
                    },
                    LivroSolicitado = s.LivroSolicitado == null ? null : new LivroResponseDTO
                    {
                        LivroId = s.LivroSolicitado.LivroId,
                        Nome = s.LivroSolicitado.Nome
                    }
                })
                .ToListAsync();

            return Result<IEnumerable<SolicitacaoEmprestimoDTOResponse>>(true, EncontradasSucesso, todosSolicitacoesEmprestimos, ResultType.Sucesso);
        }

        public async Task<ServiceResult<SolicitacaoEmprestimoDTOResponse>> CriarSolicitacaoEmprestimoAsync(SolicitacaoEmprestimoDTOCreate solicitacaoCreateDTO, ClaimsPrincipal userClaims)
        {

            var userId = ObterIdUser(userClaims);

            var usuarioIdSolicitacaoEmprestimo = await ObterUsuarioAtivoAsync(userId, false);

            var livroIdSolicitacaoEmprestimo = await ObterLivroAtivoAsync(solicitacaoCreateDTO.IdLivro, true);

            var solicitacaoAnteriorMesmoLivro = await VerificarSeExisteUmaSolicitacaoAtivaAnteriorComMesmoLivro(userId, livroIdSolicitacaoEmprestimo);

            var solicitacaoEmprestimoEntity = new SolicitacaoEmprestimo
            {
                IdLivro = livroIdSolicitacaoEmprestimo.LivroId,
                IdUsuarioEmprestimo = usuarioIdSolicitacaoEmprestimo.Id,
                Status = StatusSolicitacao.Aguardando,
                DataSolicitacao = DateTime.UtcNow,
                Ativo = true,
            };

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await _context.Solicitacoes.AddAsync(solicitacaoEmprestimoEntity);
                livroIdSolicitacaoEmprestimo.Quantidade--;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();

                return Result<SolicitacaoEmprestimoDTOResponse>(
                    false,
                    ErroAoSalvar + "Db erro: " + dbEx,
                    null,
                    ResultType.Erro
                );

            }

            return Result(true, AdicionadoSucesso, Mapear(solicitacaoEmprestimoEntity), ResultType.Criado);

        }

        public async Task<ServiceResult<SolicitacaoEmprestimoDTOResponse>> ListarSolicitacaoEmprestimoPorIdAsync(int idSolicitacatao, ClaimsPrincipal userClaim)
        {

            var isAdmin = ObterPermiss(userClaim);
            var userId = ObterIdUser(userClaim);

            var query = AplicarFiltroVisualizacao(
                _context.Solicitacoes.AsNoTracking(),
                isAdmin,
                userId
            );

            var itemDesejado = await query
                .Include(s => s.UsuarioSolicitante)
                .Include(s => s.LivroSolicitado)
                .FirstOrDefaultAsync(s => s.Id == idSolicitacatao);

            if (itemDesejado == null)
                return Result<SolicitacaoEmprestimoDTOResponse>(false, NaoEncontrado, null, ResultType.NotFound);

            return Result<SolicitacaoEmprestimoDTOResponse>(true, EncontradasSucesso, Mapear(itemDesejado), ResultType.Sucesso);

        }

        public async Task<ServiceResult<bool>> CancelarSolicitacaoEmprestimoAsync(int id)
        {

            var solicitacao = await ObterSolicitacaoAsync(id, true);

            var emprestimos = await VerificarSeExisteEmprestimoComSolicitacaoAtual(solicitacao);

            var livro = await ObterLivroAtivoAsync(solicitacao.IdLivro, true);

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                solicitacao.Ativo = false;
                livro.Quantidade++;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException dbEx)
            {
                await transaction.RollbackAsync();

                return Result<bool>(
                    false,
                    ErroAoSalvar + "Db erro: " + dbEx,
                    false,
                    ResultType.Erro
                );

            }

            return Result<bool>(true, ExcluidoSucesso, true, ResultType.Sucesso);

        }

        public async Task<ServiceResult<SolicitacaoEmprestimoDTOResponse>> AlterarStatusSolicitacaoAsync(int id, SolicitacaoEmprestimoDTOUpdateAdmin dto)
        {
            var solicitacao = await ObterSolicitacaoAsync(id);

            if (solicitacao == null)
                return Result<SolicitacaoEmprestimoDTOResponse>(false, NaoEncontrado, null, ResultType.NotFound);

            if (solicitacao.Status == StatusSolicitacao.Aprovado)
                return Result<SolicitacaoEmprestimoDTOResponse>(
                    false,
                    "Solicitações aprovadas não podem ser alteradas.",
                    null,
                    ResultType.Conflito);

            if (dto.Status == StatusSolicitacao.Aguardando)
                return Result<SolicitacaoEmprestimoDTOResponse>(
                    false,
                    "Não é permitido retornar para aguardando.",
                    null,
                    ResultType.Conflito);

            var livro = await _context.Livros.FirstOrDefaultAsync(l => l.LivroId == solicitacao.IdLivro && l.Ativo);

            if (livro == null)
                return Result<SolicitacaoEmprestimoDTOResponse>(
                    false,
                    "Livro vinculado não encontrado.",
                    null,
                    ResultType.NotFound
                );

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (solicitacao.Status != dto.Status)
                {
                    // Reprovar devolve estoque
                    if (dto.Status == StatusSolicitacao.Reprovado &&
                        solicitacao.Status == StatusSolicitacao.Aguardando)
                    {
                        livro.Quantidade++;
                    }

                    // Aprovar consome estoque
                    if (dto.Status == StatusSolicitacao.Aprovado &&
                        solicitacao.Status == StatusSolicitacao.Aguardando)
                    {
                        if (livro.Quantidade <= 0)
                            return Result<SolicitacaoEmprestimoDTOResponse>(
                                false,
                                "Sem estoque disponível.",
                                null,
                                ResultType.Conflito);

                        livro.Quantidade--;
                    }
                }


                solicitacao.Status = dto.Status;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();

                return Result<SolicitacaoEmprestimoDTOResponse>(
                    false,
                    ErroAoSalvar + " Db erro: " + ex.Message,
                    null,
                    ResultType.Erro
                );
            }

            return Result(true, AtualizadoSucesso, Mapear(solicitacao), ResultType.Atualizado);
        }

        private IQueryable<SolicitacaoEmprestimo> AplicarFiltroVisualizacao(IQueryable<SolicitacaoEmprestimo> query, bool isAdmin, string id)
        {

            if (!isAdmin)
                query = query.Where(s => s.IdUsuarioEmprestimo == id);

            return query;
        }

        private SolicitacaoEmprestimoDTOResponse Mapear(SolicitacaoEmprestimo s)
        {
            var solicitacaoMapeada = new SolicitacaoEmprestimoDTOResponse
            {
                Id = s.Id,
                IdUsuarioEmprestimo = s.IdUsuarioEmprestimo,
                IdLivro = s.IdLivro,
                DataSolicitacao = s.DataSolicitacao,
                Status = s.Status,
                UsuarioSolicitante = s.UsuarioSolicitante == null ? null : new UsuarioDTOResponse
                {
                    UsuarioId = s.UsuarioSolicitante.Id,
                    Nome = s.UsuarioSolicitante.Nome,
                    Ativo = s.UsuarioSolicitante.Ativo,
                },
                LivroSolicitado = s.LivroSolicitado == null ? null : new LivroResponseDTO
                {
                    LivroId = s.LivroSolicitado.LivroId,
                    Nome = s.LivroSolicitado.Nome,
                }
            };

            return solicitacaoMapeada;
        }

        private bool ObterPermiss(ClaimsPrincipal userClaim)
        {
            var permissao = userClaim.IsInRole("Admin");
            return permissao;

        }

        private string ObterIdUser(ClaimsPrincipal userClaim)
        {
            var userId = userClaim.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Usuário não autenticado.");

            return userId!;
        }

        private async Task<Usuario> ObterUsuarioAtivoAsync(string userId, bool editar)
        {
            IQueryable<Usuario> query = _context.Users;

            if (!editar)
                query = query.AsNoTracking();

            var usuario = await query
                .FirstOrDefaultAsync(u => u.Id == userId && u.Ativo);

            if (usuario == null)
                throw new Exception("Usuário não encontrado ou inativo.");

            return usuario;
        }

        private async Task<Livro> ObterLivroAtivoAsync(int id, bool editar)
        {
            IQueryable<Livro> query = _context.Livros;

            if (!editar)
                query = query.AsNoTracking();

            var livro = await query .FirstOrDefaultAsync(l => l.LivroId == id && l.Ativo);

            if (livro == null)
                throw new ("Livro não encontrado ou inativo.");

            if(livro.Quantidade < 1)
                throw new("Livro não possui quantidade suficiente.");

            return livro;
        }

        private async Task VerificarSeExisteUmaSolicitacaoAtivaAnteriorComMesmoLivro(string userId, Livro livro)
        {
            IQueryable<SolicitacaoEmprestimo> query = _context.Solicitacoes;

            var solicitacao = await query.FirstOrDefaultAsync(s => s.Ativo && s.IdUsuarioEmprestimo == userId && s.IdLivro == livro.LivroId);

            if (solicitacao != null)
                throw new Exception("Não é possivel realizar mais uma solicitacao do mesmo livro.");
        }

        private async Task<SolicitacaoEmprestimo> ObterSolicitacaoAsync(int id, bool apagar = false)
        {
            IQueryable<SolicitacaoEmprestimo> query = _context.Solicitacoes;

            var solicitacaoApagar = await query.FirstOrDefaultAsync(s => s.Id == id && s.Ativo == true);

            if (solicitacaoApagar == null)
                throw new Exception("Não foi localizado uma Solicitacao com esse Id.");

            if ((solicitacaoApagar.Status != StatusSolicitacao.Aguardando) && apagar)
                throw new Exception("Apenas solicitações aguardando podem ser canceladas.");

            return solicitacaoApagar;
        }

        private async Task VerificarSeExisteEmprestimoComSolicitacaoAtual(SolicitacaoEmprestimo solicitacaoEmprestimo)
        {
            IQueryable<Emprestimo> query = _context.Emprestimos;

            var emprestimos = await query.Include(e => e.SolicitacaoEmprestimo).FirstOrDefaultAsync(e => e.SolicitacaoEmprestimoId == solicitacaoEmprestimo.Id && e.SolicitacaoEmprestimo.Ativo);

            if (emprestimos != null)
                throw new Exception("Existem emprestimos vinculados a essa solicitação, não é possivel cancelar");
        }
    }
}
