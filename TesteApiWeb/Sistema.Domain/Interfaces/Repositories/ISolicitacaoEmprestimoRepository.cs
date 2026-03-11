using Biblioteca_WEB_API_REST_ASP.Models;
using System.Security.Claims;

namespace BibliotecaWebApiRest.Repositories.Interfaces
{
    public interface ISolicitacaoEmprestimoRepository : IRepository<SolicitacaoEmprestimo>
    {
        Task<IEnumerable<SolicitacaoEmprestimo>> ObterTodosPorPermissaoAsync(bool incluirInativos, bool isAdmin, string userId);
        Task <bool> ExisteSolicitacaoAtivaParaUsuarioELivroAsync(string userId, int livroId);
        Task<SolicitacaoEmprestimo?> ObterSolicitacaoEmprestimoPorId(int idSolicitacao, string userId, bool incluirInativos, bool isAdmin);

        Task<IEnumerable<SolicitacaoEmprestimo>> ObterTodasAsMinhasSolicitacoes(string userId);


    }
}
