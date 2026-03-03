using Biblioteca_WEB_API_REST_ASP.Context;
using BibliotecaWebApiRest.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Sistema.Domain.Interfaces;

namespace Sistema.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class, IEntityBase
    {

        protected readonly AppDBContextSistema _context;
        protected readonly DbSet<T> _dbSet;
        
        public Repository(AppDBContextSistema context)
        {
            _context = context;
            _dbSet = context.Set<T>();

        }

        public async Task AdicionarAsync(T entity) => await _context.Set<T>().AddAsync(entity);

        public void Atualizar(T entity) =>  _context.Set<T>().Update(entity);

        public async Task<T?> ObterPorIdAsync(int id, bool incluirInativos)
        {
            var query = _context.Set<T>().Where(e => e.Id == id);

            if (!incluirInativos)
                query = query.Where(l => l.Ativo);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> ObterTodosAsync(bool incluirInativos)
        {
            var query = _context.Set<T>().AsNoTracking();

            if (!incluirInativos)
                query = query.Where(l => l.Ativo);

            return await query.ToListAsync();

        }

        public async Task SalvarAsync() => await _context.SaveChangesAsync();

        public void SoftDelete(T entity)
        {
            if (entity is IEntityBase ativa) ativa.Ativo = false;
        }

    }
}
