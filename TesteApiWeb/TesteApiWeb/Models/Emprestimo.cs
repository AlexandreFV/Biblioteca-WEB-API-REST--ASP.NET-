
using System.ComponentModel.DataAnnotations;

namespace TesteApiWeb.Models
{
    public class Emprestimo
{
        [Key]
        public int Id { get; set; }

        [Required]
        public string UsuarioAdminId { get; set; } = string.Empty;
        public Usuario UsuarioAdminAutorizou { get; set; } = null!;

        [Required]
        public string UsuarioEmprestimoId { get; set; } = string.Empty;
        public Usuario UsuarioEmprestimo { get; set; } = null!;

        [Required]
        public int LivroId { get; set; }
        public Livro Livro { get; set; } = null!;

        [Required]
        public DateTime DataEmprestimo { get; set; }
        public DateTime DataPrevistaDevolucao { get; set; }
        public DateTime? DataDevolucao { get; set; }

        public bool Ativo => DataDevolucao == null;


    }
}
