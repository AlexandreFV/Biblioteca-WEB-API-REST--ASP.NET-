using DTOS.Categoria;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;
using TesteApiWeb.Class;
using TesteApiWeb.Context;
using TesteApiWeb.Services;
using static DTOS.Categoria.CategoriaDTO;

namespace TesteApiWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
