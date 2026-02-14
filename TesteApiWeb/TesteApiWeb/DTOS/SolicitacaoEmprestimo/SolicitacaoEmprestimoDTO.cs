using DTOS.Usuario;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Biblioteca_WEB_API_REST_ASP.Models;
using static DTOS.Livro.LivroDTO;
using static DTOS.Usuario.UsuarioDTO;

namespace DTOS.SolicitacaoEmprestimo
{
    public class SolicitacaoEmprestimoDTO
{

        public class SolicitacaoEmprestimoDTOCreate
        {
            [Required(ErrorMessage = "É obrigatorio o ID do livro a ser emprestado")]
            public int IdLivro { get; set; }

        }

        public class SolicitacaoEmprestimoDTOUpdate
        {
            [Required(ErrorMessage = "O Id do livro é obrigatório")]
            public int LivroId { get; set; }
            
            [JsonIgnore]
            public DateTime DataSolicitacao { get; set; }

        }

        public class SolicitacaoEmprestimoDTOResponse
        {
            public int Id { get; set; }

            public string IdUsuarioEmprestimo { get; set; } = string.Empty;

            public int IdLivro { get; set; }

            public DateTime DataSolicitacao { get; set; }

            public StatusSolicitacao Status { get; set; }

            public UsuarioDTOResponse? UsuarioSolicitante { get; set; }
            public LivroResponseDTO? LivroSolicitado { get; set; }

        }

        public class SolicitacaoEmprestimoDTOUpdateAdmin
        {
            [Required]
            public StatusSolicitacao Status { get; set; }

        }

    }
}
