using System.ComponentModel.DataAnnotations;
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

        public class SolicitacaoEmprestimoDTOResponse
        {
            public int Id { get; set; }

            public string IdUsuarioCliente { get; set; } = string.Empty;
            public string IdUsuarioAdmin { get; set; } = string.Empty;

            public int IdLivro { get; set; }

            public DateTime DataSolicitacao { get; set; }
            public DateTime DataAlteracaoStatus { get; set; }

            public StatusSolicitacao Status { get; set; }

            public UsuarioDTOResponse? UsuarioCliente { get; set; }
            public UsuarioDTOResponse? UsuarioAdmin { get; set; }
            public LivroResponseDTO? LivroSolicitado { get; set; }

        }

        public class SolicitacaoEmprestimoDTOUpdateAdmin
        {
            [Required]
            public StatusSolicitacao Status { get; set; }

        }

    }
}
