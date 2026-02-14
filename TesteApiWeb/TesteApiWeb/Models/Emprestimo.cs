
using System.ComponentModel.DataAnnotations;

namespace Biblioteca_WEB_API_REST_ASP.Models
{
    public class Emprestimo
{
        [Key]
        public int Id { get; set; }


        [Required]
        public int SolicitacaoEmprestimoId { get; set; }
        public SolicitacaoEmprestimo SolicitacaoEmprestimo { get; set; } = null!;

        [Required]
        public string UsuarioAdminId { get; set; } = string.Empty;
        public Usuario UsuarioAdminAutorizou { get; set; } = null!;
        public DateTime DataAceiteEmprestimo { get; set; } = DateTime.UtcNow;
        public DateTime DataPrevistaDevolucao { get; set; }
        public DateTime? DataDevolucao { get; set; }

        public bool Ativo => DataDevolucao == null;

    }
}
