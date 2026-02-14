using Biblioteca_WEB_API_REST_ASP.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca_WEB_API_REST_ASP.Context
{
    public class AppDBContextSistema : IdentityDbContext<Usuario>
    {


        public AppDBContextSistema(DbContextOptions<AppDBContextSistema> options) : base(options)
        {
        }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Livro> Livros { get; set; }
        public DbSet<SolicitacaoEmprestimo> Solicitacoes { get; set; }
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
                .HasOne(e => e.SolicitacaoEmprestimo)
                .WithMany()
                .HasForeignKey(e => e.SolicitacaoEmprestimoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SolicitacaoEmprestimo>()
                .HasOne(s => s.UsuarioSolicitante)
                .WithMany()
                .HasForeignKey(s => s.IdUsuarioEmprestimo)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SolicitacaoEmprestimo>()
                .HasOne(s => s.LivroSolicitado)
                .WithMany()
                .HasForeignKey(s => s.IdLivro)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}