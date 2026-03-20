using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Sistema.Application.Interfaces.Services;
using static DTOS.Categoria.CategoriaDTO;

namespace Biblioteca_WEB_API_REST_ASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("porUsuario")]
    public class CategoriasController : ControllerPersonalizado
    {

        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService) {_categoriaService = categoriaService;  }

        /// <summary>
        /// Listagem das categorias
        /// </summary>
        /// <returns>Categorias filtradas por permissao</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceResult<IEnumerable<CategoriaResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResult<IEnumerable<CategoriaResponseDTO>>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ServiceResult<IEnumerable<CategoriaResponseDTO>>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ObterCategoriasAsync() => RespostaCustomizada(await _categoriaService.listarAsync());

        /// <summary>
        /// Detalhes da categoria especificada por Id
        /// </summary>
        /// <param name="id">Identificador da categoria</param>
        /// <returns>Categoria especificada com uso de permissao</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ServiceResult<CategoriaResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResult<CategoriaResponseDTO>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ServiceResult<CategoriaResponseDTO>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ServiceResult<CategoriaResponseDTO>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ObterCategoriaPorId(int id) => RespostaCustomizada(await _categoriaService.buscarPorId(id));

        /// <summary>
        /// Adicao de nova categoria
        /// </summary>
        /// <param name="categoriaDTO">Dados da nova categoria</param>
        /// <returns>Nova categoria adicionada em sistema</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ServiceResult<CategoriaResponseDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ServiceResult<CategoriaResponseDTO>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ServiceResult<CategoriaResponseDTO>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ServiceResult<CategoriaResponseDTO>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ServiceResult<CategoriaResponseDTO>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AdicionarCategoriaAsync(CategoriaCreateDTO categoriaDTO) 
        {
            var result = await _categoriaService.adicionarAsync(categoriaDTO);

            if (result.Sucesso)
            {
                return CreatedAtAction(nameof(ObterCategoriaPorId), new { id = result.Dados!.Id }, result.Dados);
            }

            return RespostaCustomizada(result);

        }

        /// <summary>
        /// Edicao da categoria especificada por Id
        /// </summary>
        /// <param name="id">Identificador de categoria</param>
        /// <param name="categoriaDTO">Novos dados da categoria</param>
        /// <returns>Categoria editada com novas informacoes</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> EditarCategoriaAsync(int id, CategoriaUpdateDTO categoriaDTO) => RespostaCustomizada(await _categoriaService.editarAsync(id, categoriaDTO));

        /// <summary>
        /// Alterado status "ativo" da categoria
        /// </summary>
        /// <param name="id">Identificador da categoria</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ExcluirCategoriaAsync(int id) => RespostaCustomizada(await _categoriaService.softDeletePorId(id));
            
    }
}
