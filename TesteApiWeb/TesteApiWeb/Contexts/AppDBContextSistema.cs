using Microsoft.EntityFrameworkCore;
using TesteApiWeb.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TesteApiWeb.Context
{
    public class AppDBContextSistema : IdentityDbContext<Usuario>
    {


        public AppDBContextSistema(DbContextOptions<AppDBContextSistema> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Livro> Livros { get; set; }
        public DbSet<Emprestimo> Emprestimos { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Emprestimo>()
                .HasOne(e => e.UsuarioAdminAutorizou)
                .WithMany()
                .HasForeignKey(e => e.UsuarioAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Emprestimo>()
                .HasOne(e => e.UsuarioEmprestimo)
                .WithMany()
                .HasForeignKey(e => e.UsuarioEmprestimoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}