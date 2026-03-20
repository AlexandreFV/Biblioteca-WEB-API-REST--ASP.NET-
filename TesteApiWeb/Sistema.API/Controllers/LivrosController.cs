using Biblioteca_WEB_API_REST_ASP;
using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Models;
using DTOS.Livro;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Sistema.Application.Interfaces.Services;
using System.Security.Claims;
using TesteApiWeb.Services;
using static DTOS.Livro.LivroDTO;

namespace Biblioteca_WEB_API_REST_ASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting("porUsuario")]
    public class LivrosController : ControllerPersonalizado
    {

        private readonly ILivroService _livroService;

        public LivrosController(ILivroService livroService) { _livroService = livroService; }

        /// <summary>
        /// Listagem dos livros
        /// </summary>
        /// <returns>Livros filtrados por permissao</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceResult<IEnumerable<LivroResponseDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObterLivrosAsync() => RespostaCustomizada(await _livroService.listarAsync());

        /// <summary>
        /// Detalhes do livro especificado por Id
        /// </summary>
        /// <param name="id">Identificador do livro</param>
        /// <returns>Livro especificada com uso de permissao</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ServiceResult<LivroResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResult<LivroResponseDTO>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObterLivroPorId(int id) => RespostaCustomizada(await _livroService.buscarPorId(id));

        /// <summary>
        /// Adicao de novo livro
        /// </summary>
        /// <param name="livroCreateEditDTO">Dados do novo livro</param>
        /// <returns>Novo livro adicionado em sistema</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ServiceResult<LivroResponseDTO>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ServiceResult<LivroResponseDTO>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ServiceResult<LivroResponseDTO>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ServiceResult<LivroResponseDTO>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ServiceResult<LivroResponseDTO>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ServiceResult<LivroResponseDTO>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CriarLivroAsync(LivroCreateDTO livroCreateEditDTO)
        {

            var result = await _livroService.adicionarAsync(livroCreateEditDTO);

            if (result.Sucesso)
                return CreatedAtAction(nameof(ObterLivroPorId), new { id = result.Dados!.Id }, result.Dados);

            return RespostaCustomizada(result);
        }

        /// <summary>
        /// Edicao do livro especificado por Id
        /// </summary>
        /// <param name="id">Identificador de livro</param>
        /// <param name="livroDto">Novos dados do livro</param>
        /// <returns>Livro editado com novas informacoes</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> EditarLivroAsync(int id, LivroEditDTO livroDto) => RespostaCustomizada(await _livroService.editarAsync(id, livroDto));

        /// <summary>
        /// Alterado status "ativo" do livro
        /// </summary>
        /// <param name="id">Identificador do livro</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ExcluirLivroAsync(int id) => RespostaCustomizada(await _livroService.softDeletePorId(id));
    }
}
