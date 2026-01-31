using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TesteApiWeb.Models
{
    public class Categoria
    {

        [Key]
        public int CategoriaId { get; set; }

        public String Nome { get; set; } = string.Empty;

        //public DateTime DataCriacao { get; set; }
        //public int IdUsuarioCriacao { get; set; }

        public ICollection<Livro>? Livros { get; set; } = new Collection<Livro>();
        public bool Ativo { get; set; }

    }
}
