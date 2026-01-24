using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TesteApiWeb.Models
{
    public class CategoriaDTO
    {

        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "O nome da categoria é obrigatorio")]
        public String Nome { get; set; } = string.Empty;


    }
}
