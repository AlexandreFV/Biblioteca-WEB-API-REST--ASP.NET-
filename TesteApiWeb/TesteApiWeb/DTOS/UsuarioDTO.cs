using System.ComponentModel.DataAnnotations;

namespace TesteApiWeb.Models
{
    public class UsuarioDTO
    {

        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "O Nome de usuario é obrigatorio")]
        public String Nome { get; set; } = String.Empty;

        [Required(ErrorMessage = "A senha de usuario é obrigatorio")]
        public String Senha { get; set; } = String.Empty;

        [Required(ErrorMessage = "O tipo de usuario é obrigatorio")]
        public String Tipo { get; set; } = String.Empty;
        public String Ativo { get; set; } = string.Empty;

    }
}
