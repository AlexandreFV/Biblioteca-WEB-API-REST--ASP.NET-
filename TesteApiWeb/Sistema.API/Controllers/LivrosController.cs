using Biblioteca_WEB_API_REST_ASP;
using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Models;
using DTOS.Livro;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema.Application.Interfaces.Services;
using System.Security.Claims;
using TesteApiWeb.Services;
using static DTOS.Livro.LivroDTO;

namespace Biblioteca_WEB_API_REST_ASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LivrosController : ControllerPersonalizado
    {

        private readonly ILivroService _livroService;

        public LivrosController(ILivroService livroService) { _livroService = livroService; }

        [HttpGet]
        public async Task<IActionResult> ObterLivrosAsync() => RespostaCustomizada(await _livroService.listarAsync());


        [HttpGet("{id}")]
        public async Task<IActionResult> ObterLivroPorId(int id) => RespostaCustomizada(await _livroService.buscarPorId(id));
        
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CriarLivroAsync(LivroCreateDTO livroCreateEditDTO)
        {

            var result = await _livroService.adicionarAsync(livroCreateEditDTO);

            if (result.Sucesso)
                return CreatedAtAction(nameof(ObterLivroPorId), new { id = result.Dados!.Id }, result.Dados);

            return RespostaCustomizada(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditarLivroAsync(int id, LivroEditDTO livroDto) => RespostaCustomizada(await _livroService.editarAsync(id, livroDto));

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExcluirLivroAsync(int id) => RespostaCustomizada(await _livroService.softDeletePorId(id));
    }
}
