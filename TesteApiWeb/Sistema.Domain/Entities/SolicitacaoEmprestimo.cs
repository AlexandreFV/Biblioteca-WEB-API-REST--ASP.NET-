using Sistema.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca_WEB_API_REST_ASP.Models
{
    public class SolicitacaoEmprestimo : IEntityBase
    {

        [Key]
        public int Id { get; set; }

        public string IdUsuarioCliente { get; set; } = string.Empty;

        public int IdLivro { get; set; }
        public string IdUsuarioAdmin { get; set; } = string.Empty;

        public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;

        public StatusSolicitacao Status { get; set; }

        public DateTime DataAlteracaoStatus { get; set; }

        [ForeignKey(nameof(IdUsuarioCliente))]
        public Usuario? UsuarioCliente { get; set; }

        [ForeignKey(nameof(IdUsuarioAdmin))]
        public Usuario? UsuarioAdmin { get; set; }

        [ForeignKey(nameof(IdLivro))]
        public Livro? LivroSolicitado { get; set; }

        public bool Ativo {  get; set; }
    }

    public enum StatusSolicitacao { Aprovado, Reprovado, Aguardando}
}
