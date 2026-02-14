using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static DTOS.Categoria.CategoriaDTO;

namespace Biblioteca_WEB_API_REST_ASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CategoriasController : ControllerPersonalizado
    {

        private readonly CategoriaService _categoriaService;

        public CategoriasController(CategoriaService categoriaService) {_categoriaService = categoriaService;  }

        [HttpGet]
        public async Task<IActionResult> ObterCategoriasAsync() => RespostaCustomizada(await _categoriaService.ListarCategoriasAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterCategoriaPorId(int id) => RespostaCustomizada(await _categoriaService.ProcurarCategoriaAsync(id));

        [HttpPost]
        public async Task<IActionResult> AdicionarCategoriaAsync(CategoriaCreateDTO categoriaDTO) 
        {
            var result = await _categoriaService.CriarCategoriaAsync(categoriaDTO);

            if (result.Sucesso)
            {
                return CreatedAtAction(nameof(ObterCategoriaPorId), new { id = result.Dados!.CategoriaId }, result.Dados);
            }

            return RespostaCustomizada(result);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarCategoriaAsync(int id, CategoriaUpdateDTO categoriaDTO) => RespostaCustomizada(await _categoriaService.EditarCategoriaAsync(id, categoriaDTO));

        [HttpDelete("{id}")]
        public async Task<IActionResult> ExcluirCategoriaAsync(int id) => RespostaCustomizada(await _categoriaService.ApagarCategoriaAsync(id));
            
    }
}
