using System.ComponentModel.DataAnnotations;

namespace DTOS.Emprestimo
{
    public class EmprestimoDTO
{

        public class EmprestimoDTOCreate
        {
            [Required(ErrorMessage = "O Id do livro é obrigatório")]
            public int LivroId { get; set; }

            [Required(ErrorMessage = "O Id do usuario a quem será emprestado é obrigatório")]
            public int UsuarioEmprestimoId { get; set; }

            [Required(ErrorMessage = "A data prevista para devolucao é obrigatorio")]
            public DateTime DataPrevistaDevolucao { get; set; }

        }

        public class EmprestimoDTOUpdate
        {
            [Required(ErrorMessage = "O Id do livro é obrigatório")]
            public int LivroId { get; set; }

            [Required(ErrorMessage = "O Id do usuario a quem será emprestado é obrigatório")]
            public int UsuarioEmprestimoId { get; set; }

            [Required(ErrorMessage = "A data prevista para devolucao é obrigatorio")]
            public DateTime DataPrevistaDevolucao { get; set; }
            public DateTime? DataDevolucao { get; set; }

        }

        public class EmprestimoDTOResponse
        {
            public int EmprestimoId { get; set; }
            public DateTime DataEmprestimo { get; set; }
            public DateTime DataPrevistaDevolucao { get; set; }
            public DateTime? DataDevolucao { get; set; }

            public int UsuarioEmprestimoId { get; set; }
            public string NomeUsuarioEmprestimo { get; set; } = string.Empty;

            public int UsuarioAdminId { get; set; }
            public string NomeUsuarioAdmin { get; set; } = string.Empty;

            public int LivroId { get; set; }
            public string NomeLivro { get; set; } = String.Empty;

        }
    }
}
