using Sistema.Domain.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Biblioteca_WEB_API_REST_ASP.Models
{
    public class Categoria : IEntityBase
    {

        [Key]
        public int Id { get; set; }

        public string Nome { get; set; }

        public DateTime DataCriacao { get; set; }
        public string IdUsuarioCriacao { get; set; }

        public ICollection<Livro>? Livros { get; set; }
        public bool Ativo { get; set; }


        public Categoria() { }

        public Categoria(string nome, string idUsuarioCriacao)
        {

            if (string.IsNullOrWhiteSpace(nome) || nome.Trim().Length < 3)
                throw new Exception("Nome deve ter no mínimo 3 caracteres");

            if (string.IsNullOrWhiteSpace(idUsuarioCriacao))
                throw new Exception("O Id do usuario Admin é obrigatorio");

            Nome = nome.Trim();
            Ativo = true;
            DataCriacao = DateTime.UtcNow;
            IdUsuarioCriacao = idUsuarioCriacao;
            Livros = new Collection<Livro>();
        }
    }
}
