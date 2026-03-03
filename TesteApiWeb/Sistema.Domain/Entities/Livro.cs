using Sistema.Domain.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Biblioteca_WEB_API_REST_ASP.Models
{
    public class Livro : IEntityBase
    {

        [Key]
        public int Id { get; set; }

        public string Nome { get; set; }

        public int Quantidade { get; set; }

        public DateTime DataCriacao { get; set; }
        public string IdUsuarioAdminCriou { get; set; }
        
        [ForeignKey(nameof(IdUsuarioAdminCriou))]
        public Usuario? UsuarioAdmin { get; set; }

        public ICollection<Categoria> Categorias { get; set; }
        public bool Ativo { get; set; }

        public Livro() { }

        public Livro(string nome, int quantidade, string idUsuarioAdminCriou) 
        {
            if (string.IsNullOrWhiteSpace(nome) || nome.Trim().Length < 3)
                throw new Exception("Nome deve ter no mínimo 3 caracteres");

            if (string.IsNullOrWhiteSpace(IdUsuarioAdminCriou))
                throw new Exception("O Id do usuario Admin é obrigatorio");

            if (int.IsNegative(quantidade) || quantidade.Equals(0))
                throw new Exception("A quantidade deve ser maior que 0");

            Nome = nome.Trim();
            Quantidade = quantidade;
            Ativo = true;
            Categorias = new Collection<Categoria>();
            IdUsuarioAdminCriou = idUsuarioAdminCriou;
            DataCriacao = DateTime.UtcNow;

        }

    }
}
