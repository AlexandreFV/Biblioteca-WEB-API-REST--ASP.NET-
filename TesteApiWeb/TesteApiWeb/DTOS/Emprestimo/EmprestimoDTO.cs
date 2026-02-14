using Biblioteca_WEB_API_REST_ASP.Models;
using System.ComponentModel.DataAnnotations;
using static DTOS.SolicitacaoEmprestimo.SolicitacaoEmprestimoDTO;
using static DTOS.Usuario.UsuarioDTO;

namespace DTOS.Emprestimo
{
    public class EmprestimoDTO
{

        public class EmprestimoDTOCreate
        {

            [Required]
            public int SolicitacaoEmprestimoId { get; set; }
            [Required]
            public string UsuarioAdminId { get; set; } = string.Empty;
            public DateTime DataPrevistaDevolucao { get; set; }

        }

        public class EmprestimoDTODevolucao
        {
            [Required]
            public DateTime DataDevolucao { get; set; }

        }

        public class EmprestimoDTOResponse
        {

            public int SolicitacaoEmprestimoId { get; set; }
            public SolicitacaoEmprestimoDTOResponse SolicitacaoEmprestimo { get; set; } = null!;

            public string UsuarioAdminId { get; set; } = string.Empty;
            public UsuarioDTOResponse UsuarioAdminAutorizou { get; set; } = null!;
            public DateTime DataAceiteEmprestimo { get; set; } = DateTime.UtcNow;
            public DateTime DataPrevistaDevolucao { get; set; }
            public DateTime? DataDevolucao { get; set; }

            public bool Ativo => DataDevolucao == null;

        }
    }
}
