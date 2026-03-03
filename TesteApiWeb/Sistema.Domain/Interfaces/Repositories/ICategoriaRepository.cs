using Biblioteca_WEB_API_REST_ASP.Models;

namespace BibliotecaWebApiRest.Repositories.Interfaces
{
    public interface ICategoriaRepository : IRepository<Categoria>
    {

        Task<bool> ExisteCategoriaComEsseNomeAsync(string categoriaNome, int? ignorarId = null);
        Task<bool> ExisteCategoriaVinculadaAoLivroAsync(int idCategoria);
        Task<ICollection<Categoria>> ObterCategoriasValidasAsync(ICollection<int> ids);
    }
}
