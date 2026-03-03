using Sistema.Domain.Interfaces;

namespace BibliotecaWebApiRest.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> ObterPorIdAsync(int id, bool incluirInativos);
        Task<IEnumerable<T>> ObterTodosAsync(bool incluirInativos);
        Task AdicionarAsync(T entity);
        void Atualizar(T entity);
        Task SalvarAsync();
        void SoftDelete(T entity);
    }
}
