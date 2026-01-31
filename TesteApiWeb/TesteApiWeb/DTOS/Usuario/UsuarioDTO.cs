using System.ComponentModel.DataAnnotations;

namespace DTOS.Usuario
{
    public class UsuarioDTO
    {

        public class UsuarioDTOCreate
        {
            [Required]
            public string Nome { get; set; } = string.Empty;

            [Required]
            public string Senha { get; set; } = string.Empty;

            [Required]
            public string Email { get; set; } = string.Empty;

        }

        public class UsuarioDTOEdit
        {
            [Required]
            public string Nome { get; set; } = string.Empty;

            [Required]
            public string Email { get; set; } = string.Empty;
            
            [Required]
            public bool Ativo { get; set; }

        }

        public class UsuarioDTOResponse
        {
            public string UsuarioId { get; set; }

            public String Nome { get; set; } = String.Empty;

            public bool Ativo { get; set; }

        }

        public class UsuarioDTOLogin
        {
            [Required]
            public string UsuarioId { get; set; }

            [Required]
            public string Nome { get; set; } = string.Empty;

            [Required]
            public bool Ativo { get; set; }

        }
    }
}
