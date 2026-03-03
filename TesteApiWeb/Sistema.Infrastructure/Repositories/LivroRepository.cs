using Biblioteca_WEB_API_REST_ASP.Context;
using Biblioteca_WEB_API_REST_ASP.Models;
using BibliotecaWebApiRest.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Sistema.Infrastructure.Repositories;
using System;

namespace BibliotecaWebApiRest.Repositories.Concretas
{
    public class LivroRepository : Repository<Livro>, ILivroRepository
    {
        public LivroRepository(AppDBContextSistema context) : base(context) { }

        public async Task<bool> ExisteLivroComEsseNomeAsync(string livroNome, int? ignorarId = null)
        {
            return await _context.Livros
                .AsNoTracking()
                .AnyAsync(l => l.Nome == livroNome && (!ignorarId.HasValue || l.Id != ignorarId));
        }

        public async Task<bool> ExisteSolicitacaoAtivaParaLivroAsync(int idLivro)
        {
            return await _context.Solicitacoes
                .AsNoTracking()
                .AnyAsync(s => s.IdLivro == idLivro && s.Ativo);
        }

        public async Task<Livro?> ObterLivroIncluindoCategoriaPorId(int idLivro, bool incluirInativos)
        {
            var query =  _context.Livros.Include(c => c.Categorias);

            if (!incluirInativos)
                return await query.FirstOrDefaultAsync(l => l.Id == idLivro && l.Ativo);

            return await query.FirstOrDefaultAsync(l => l.Id == idLivro);
        }

        public async Task<IEnumerable<Livro>> ObterTodosLivrosIncluindoCategoria(bool incluirInativos)
        {
            var query = _context.Livros.Include(c => c.Categorias);

            if (!incluirInativos)
                return await query.Where(l => l.Ativo).ToListAsync();

            return await query.ToListAsync();
        }
    }
}
