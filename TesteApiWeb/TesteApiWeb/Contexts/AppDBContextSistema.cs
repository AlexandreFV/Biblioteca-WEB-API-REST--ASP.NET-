using Microsoft.EntityFrameworkCore;
using TesteApiWeb.Models;

namespace TesteApiWeb.Context
{
    public class AppDBContextSistema : DbContext
    {


        public AppDBContextSistema(DbContextOptions<AppDBContextSistema> options) : base (options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Livro> Livros { get; set; }



    }
}
