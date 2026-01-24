using System.ComponentModel.DataAnnotations;
using TesteApiWeb.Models;

namespace TesteApiWeb.DTOS
{
    public class LivroDTOCreateEdit
    {

        public int LivroId { get; set; }

        [Required(ErrorMessage = "O nome do livro é obrigatorio")]
        public String Nome { get; set; } = String.Empty;

        [Required(ErrorMessage = "A quantidade do livro é obrigatorio")]
        public int Quantidade { get; set; }

        public ICollection<int>? CategoriasIds { get; set; }

        public String Ativo { get; set; } = string.Empty;

    }
}
