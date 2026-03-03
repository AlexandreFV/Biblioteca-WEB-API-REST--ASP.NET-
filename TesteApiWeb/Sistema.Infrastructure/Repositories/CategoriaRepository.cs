using Biblioteca_WEB_API_REST_ASP.Context;
using Biblioteca_WEB_API_REST_ASP.Models;
using BibliotecaWebApiRest.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Sistema.Infrastructure.Repositories;

namespace BibliotecaWebApiRest.Repositories.Concretas
{
    public class CategoriaRepository : Repository<Categoria>, ICategoriaRepository
    {
        public CategoriaRepository (AppDBContextSistema context) : base (context) { }

        public async Task<bool> ExisteCategoriaComEsseNomeAsync(string categoriaNome, int? ignorarId = null)
        {
            return await _context.Categorias
                .AsNoTracking()
                .AnyAsync(c => c.Nome == categoriaNome && (!ignorarId.HasValue || c.Id != ignorarId));

        }

        public async Task<bool> ExisteCategoriaVinculadaAoLivroAsync(int idCategoria)
        {
            return await _context.Categorias
                .AsNoTracking()
                .AnyAsync(c => c.Id == idCategoria && c.Livros.Any(l => l.Ativo));
        }

        public async Task<ICollection<Categoria>> ObterCategoriasValidasAsync(ICollection<int> ids)
        {
            return await _context.Categorias
                .Where(c => ids.Contains(c.Id) && c.Ativo)
                .ToListAsync();
        }

    }
}
