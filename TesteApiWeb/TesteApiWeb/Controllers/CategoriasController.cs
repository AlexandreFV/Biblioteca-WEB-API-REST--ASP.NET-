using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;
using TesteApiWeb.Class;
using TesteApiWeb.Context;
using TesteApiWeb.Models;
using TesteApiWeb.Services;

namespace TesteApiWeb.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerPersonalizado
    {

        private readonly CategoriaService _categoriaService;

        public CategoriasController(CategoriaService categoriaService) {_categoriaService = categoriaService;  }

        [HttpGet]
        public IActionResult ObterCategorias() => RespostaCustomizada(_categoriaService.ListarCategorias());

        [HttpGet("{id}")]
        public IActionResult ObterCategoriaPorId(int id) => RespostaCustomizada(_categoriaService.ProcurarCategoria(id));

        [HttpPost]
        public IActionResult AdicionarCategoria(CategoriaDTO categoriaDTO) 
        {
            var result = _categoriaService.CriarCategoria(categoriaDTO);

            if (result.Sucesso)
            {
                return CreatedAtAction(nameof(ObterCategoriaPorId), new { id = result.Dados!.CategoriaId }, result.Dados);
            }

            return RespostaCustomizada(result);

        }

        [HttpPut("{id}")]
        public IActionResult EditarCategoria(int id, CategoriaDTO categoriaDTO) => RespostaCustomizada(_categoriaService.EditarCategoria(id, categoriaDTO));

        [HttpDelete("{id}")]
        public IActionResult ExcluirCategoria(int id) => RespostaCustomizada(_categoriaService.ApagarCategoria(id));
            
    }
}
