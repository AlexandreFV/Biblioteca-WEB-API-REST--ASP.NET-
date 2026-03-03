using Biblioteca_WEB_API_REST_ASP.Models;

namespace BibliotecaWebApiRest.Repositories.Interfaces
{
    public interface ILivroRepository : IRepository<Livro>
    {
        Task<bool> ExisteLivroComEsseNomeAsync(string livroNome, int? ignorarId = null);
        Task<bool> ExisteSolicitacaoAtivaParaLivroAsync(int idLivro);
        Task<Livro?> ObterLivroIncluindoCategoriaPorId(int idLivro, bool incluirInativos);
        Task<IEnumerable<Livro>> ObterTodosLivrosIncluindoCategoria(bool incluirInativos);
    }
}
