using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TesteApiWeb.Models
{
    public class LivroDTOGetGetId
    {

        public int LivroId { get; set; }

        public String Nome { get; set; } = String.Empty;

        [Required(ErrorMessage = "A quantidade do livro é obrigatorio")]
        public int Quantidade { get; set; }

        public ICollection<CategoriaDTO>? Categorias { get; set; }
    }
}
