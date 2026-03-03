using Biblioteca_WEB_API_REST_ASP.Context;
using Biblioteca_WEB_API_REST_ASP.Models;
using BibliotecaWebApiRest.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Sistema.Infrastructure.Repositories;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BibliotecaWebApiRest.Repositories.Concretas
{
    public class SolicitacaoEmprestimoRepository : Repository<SolicitacaoEmprestimo>, ISolicitacaoEmprestimoRepository
    {
        public SolicitacaoEmprestimoRepository(AppDBContextSistema context) : base (context) {  }

        public async Task<bool> ExisteSolicitacaoAtivaParaUsuarioELivroAsync(string userId, int idLivro)
        {
            return await _context.Solicitacoes
                .AsNoTracking()
                .AnyAsync(s => s.IdLivro == idLivro && s.IdUsuarioCliente == userId && s.Ativo);
        }


        public async Task<SolicitacaoEmprestimo?> ObterSolicitacaoEmprestimoPorId(int idSolicitacao, string userId, bool incluirInativos, bool isAdmin)
        {
            var query = _context.Solicitacoes
                .Include(q => q.LivroSolicitado)
                .Include(q => q.UsuarioAdmin)
                .Include(q => q.UsuarioAdmin)
                .AsNoTracking()
                .Where(s => s.Id == idSolicitacao);

            if (!isAdmin)
                 query = query.Where(s => s.IdUsuarioCliente == userId);

            if (!incluirInativos)
                 query = query.Where(s => s.Ativo);


            return await query.FirstOrDefaultAsync();

        }

        public async Task<IEnumerable<SolicitacaoEmprestimo>> ObterTodosPorPermissaoAsync(bool incluirInativos, bool isAdmin, string userId)
        {
            var query = _context.Solicitacoes.Include(s => s.LivroSolicitado).Include(s => s.UsuarioAdmin).Include(s => s.UsuarioCliente).AsNoTracking();

            if (!incluirInativos)
                query = query.Where(s => s.Ativo);

            if (!isAdmin)
                query = query.Where(s => s.IdUsuarioCliente == userId);

            return await query.ToListAsync();

        }
    }
}
