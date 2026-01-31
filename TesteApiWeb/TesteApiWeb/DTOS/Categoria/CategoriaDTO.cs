using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DTOS.Categoria
{
    public record CategoriaDTO
    {

        public class CategoriaCreateDTO
        {
            [Required(ErrorMessage = "O nome da categoria é obrigatório")]
            public string Nome { get; set; } = string.Empty;
        }

        public class CategoriaUpdateDTO
        {
            [Required(ErrorMessage = "O nome da categoria é obrigatório")]
            public string Nome { get; set; } = string.Empty;
        }


        public class CategoriaResponseDTO
        {
            public int CategoriaId { get; set; }
            public string Nome { get; set; } = string.Empty;
        }

    }
}
