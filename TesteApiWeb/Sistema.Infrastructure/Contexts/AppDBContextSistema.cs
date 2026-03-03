using Biblioteca_WEB_API_REST_ASP.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SolicitacaoEmprestimo>()
                .HasOne(e => e.UsuarioAdmin)
                .WithMany()
                .HasForeignKey(e => e.IdUsuarioAdmin)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SolicitacaoEmprestimo>()
                .HasOne(e => e.UsuarioCliente)
                .WithMany()
                .HasForeignKey(e => e.IdUsuarioCliente)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SolicitacaoEmprestimo>()
                .HasOne(s => s.LivroSolicitado)
                .WithMany()
                .HasForeignKey(s => s.IdLivro)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}