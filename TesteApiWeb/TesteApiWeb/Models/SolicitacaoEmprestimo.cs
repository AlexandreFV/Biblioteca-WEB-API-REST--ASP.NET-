using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca_WEB_API_REST_ASP.Models
{
    public class SolicitacaoEmprestimo
{

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O id do usuario solicitante do emprestimo é obrigatorio")]
        public string IdUsuarioEmprestimo { get; set; } = string.Empty;

        [Required(ErrorMessage = "É obrigatorio o ID do livro a ser emprestado")]
        public int IdLivro { get; set; }

        public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;

        public StatusSolicitacao Status { get; set; }

        [ForeignKey(nameof(IdUsuarioEmprestimo))]
        public Usuario? UsuarioSolicitante { get; set; }
        [ForeignKey(nameof(IdLivro))]
        public Livro? LivroSolicitado { get; set; }

        public bool Ativo {  get; set; }
    }

    public enum StatusSolicitacao { Aprovado, Reprovado, Aguardando}
}
