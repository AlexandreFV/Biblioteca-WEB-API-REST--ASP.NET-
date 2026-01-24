using System.ComponentModel.DataAnnotations;

namespace TesteApiWeb.Models
{
    public class Usuario
    {

        [Key]
        public int UsuarioId { get; set; }

        public String Nome { get; set; } = String.Empty;

        public String Senha { get; set; } = String.Empty;

        public String Tipo { get; set; } = String.Empty;
        public String Ativo { get; set; } = string.Empty;

    }
}
