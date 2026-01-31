using DTOS.Usuario;
using System.ComponentModel.DataAnnotations;
using static DTOS.Usuario.UsuarioDTO;

namespace TesteApiWeb.Models
{
    public class AuthDTO
{
        public class LoginDTO
        {
            [Required(ErrorMessage = "É obrigatorio a inserção do e-mail")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "É obrigatorio a inserção da senha")]
            public string Senha { get; set; } = string.Empty;

        }

        public class LoginResponseDTO
        {
            [Required]
            public string Token { get; set; }

            [Required]
            public UsuarioDTOLogin UsuarioDTO { get; set; }
        }

        public class RegisterDTOCreate
        {
            [Required(ErrorMessage = "É obrigatorio a inserção do Nome")]
            public string Nome { get; set; } = string.Empty;

            [Required(ErrorMessage = "É obrigatorio a inserção da senha")]
            public string Senha { get; set; } = string.Empty;

            [Required(ErrorMessage = "É obrigatorio a inserção do e-mail")]
            public string Email { get; set; } = string.Empty;
           
            [Required]
            [RegularExpression("Admin|User")]
            public string Role { get; set; } = "User";

        }

        public class RegisterDTOResponse 
        {
            public string UsuarioId { get; set; }

            public String Nome { get; set; } = String.Empty;

            public bool Ativo { get; set; }

        }

    }
}
