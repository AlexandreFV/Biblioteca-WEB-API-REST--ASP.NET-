using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Biblioteca_WEB_API_REST_ASP.Models
{
    public class Livro
    {

        [Key]
        public int LivroId { get; set; }

        public String Nome { get; set; } = String.Empty;

        public int Quantidade { get; set; }

        //public DateTime DataCriacao { get; set; }
        //public int IdUsuarioCriacao { get; set; }
        public ICollection<Categoria>? Categorias { get; set; }
        public bool Ativo { get; set; }

    }
}
