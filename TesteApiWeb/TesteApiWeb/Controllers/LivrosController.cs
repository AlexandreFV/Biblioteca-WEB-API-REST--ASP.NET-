using DTOS.Livro;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TesteApiWeb.Class;
using TesteApiWeb.Models;
using TesteApiWeb.Services;
using static DTOS.Livro.LivroDTO;

namespace TesteApiWeb.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class LivrosController : ControllerPersonalizado
    {

        private readonly LivroService _livroService;

        public LivrosController(LivroService livroService) { _livroService = livroService; }

        [HttpGet]
        public async Task<IActionResult> ObterLivrosAsync() => RespostaCustomizada(await _livroService.ListarLivrosAsync());


        [HttpGet("{id}")]
        public async Task<IActionResult> ObterLivroPorId(int id) => RespostaCustomizada(await _livroService.ProcurarLivroPorIdAsync(id));
        
        [HttpPost]
        public async Task<IActionResult> CriarLivroAsync(LivroCreateDTO livroCreateEditDTO)
        {

            var result = await _livroService.CriarLivroAsync(livroCreateEditDTO);

            if (result.Sucesso)
                return CreatedAtAction(nameof(ObterLivroPorId), new { id = result.Dados!.LivroId }, result.Dados);

            return RespostaCustomizada(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarLivroAsync(int id, LivroEditDTO livroDto) => RespostaCustomizada(await _livroService.EditarLivroAsync(id, livroDto));

        [HttpDelete("{id}")]
        public async Task<IActionResult> ExcluirLivroAsync(int id) => RespostaCustomizada(await _livroService.ApagarLivroAsync(id));
    }
}
