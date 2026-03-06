using DTOS.Categoria;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static DTOS.Categoria.CategoriaDTO;

namespace DTOS.Livro
{
    public class LivroDTO
    {

        public class LivroCreateDTO
        {
            [Required(ErrorMessage = "O nome do livro é obrigatorio")]
            public String Nome { get; set; }

            [Required(ErrorMessage = "A quantidade de livro é obrigatorio")]
            public int Quantidade { get; set; }

            [Required(ErrorMessage = "É obrigatorio adicionar categorias ao livro")]
            public ICollection<int> CategoriasId { get; set; } = new List<int>();
        }

        public class LivroEditDTO
        {

            [Required(ErrorMessage = "O nome do livro é obrigatorio")]
            public String Nome { get; set; }

            [Required(ErrorMessage = "A quantidade de livro é obrigatorio")]
            public int Quantidade { get; set; }

            [Required(ErrorMessage = "É obrigatorio adicionar categorias ao livro")]
            public ICollection<int> CategoriasId { get; set; } = new List<int>();

        }

        public class LivroResponseDTO
        {
            public int Id { get; set; }

            public string Nome { get; set; }

            public int Quantidade { get; set; }

            public ICollection<CategoriaResponseDTO> Categorias { get; set; } = new List<CategoriaResponseDTO>();

        }
    }
}
