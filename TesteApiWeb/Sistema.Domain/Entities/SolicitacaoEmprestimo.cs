using Sistema.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Biblioteca_WEB_API_REST_ASP.Models
{
    public class SolicitacaoEmprestimo : IEntityBase
    {

        [Key]
        public int Id { get; set; }

        public string IdUsuarioCliente { get; set; }

        public int IdLivro { get; set; }
        public string? IdUsuarioAdmin { get; set; }

        public DateTime DataSolicitacao { get; set; }
        public DateTime DataUltimaAtualizacao { get; set; }

        public StatusSolicitacao Status { get; set; }

        public DateTime DataAlteracaoStatus { get; set; }

        [ForeignKey(nameof(IdUsuarioCliente))]
        public Usuario? UsuarioCliente { get; set; }

        [ForeignKey(nameof(IdUsuarioAdmin))]
        public Usuario? UsuarioAdmin { get; set; }

        [ForeignKey(nameof(IdLivro))]
        public Livro? LivroSolicitado { get; set; }

        public bool Ativo {  get; set; }

        public SolicitacaoEmprestimo() { }

        public SolicitacaoEmprestimo(string idUsuarioCliente, int idLivro)
        {

            if(string.IsNullOrWhiteSpace(idUsuarioCliente))
                throw new Exception("É obrigatorio que o id de usuario do cliente esteja vinculada ao emprestimo");

            if (int.IsNegative(idLivro) || idLivro.Equals(0))
                throw new Exception("É obrigatorio que o id do livro seja maior que 0");

            IdUsuarioCliente = idUsuarioCliente;
            IdUsuarioAdmin = null;
            IdLivro = idLivro;
            DataSolicitacao = DateTime.UtcNow;
            DataUltimaAtualizacao = DateTime.UtcNow;
            Status = StatusSolicitacao.Aguardando;
            DataAlteracaoStatus = DateTime.UtcNow;
            Ativo = true;
        }
    }

    public enum StatusSolicitacao { Aprovado, Reprovado, Aguardando}
}
