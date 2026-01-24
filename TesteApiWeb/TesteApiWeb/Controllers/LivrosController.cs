using Microsoft.AspNetCore.Mvc;
using TesteApiWeb.Class;
using TesteApiWeb.DTOS;
using TesteApiWeb.Models;
using TesteApiWeb.Services;

namespace TesteApiWeb.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class LivrosController : ControllerPersonalizado
    {

        private readonly LivroService _livroService;

        public LivrosController(LivroService livroService) { _livroService = livroService; }

        [HttpGet]
        public IActionResult ObterLivros() => RespostaCustomizada(_livroService.ListarLivros());


        [HttpGet("{id}")]
        public IActionResult ObterLivroPorId(int id) => RespostaCustomizada(_livroService.ProcurarLivroPorId(id));
        
        [HttpPost]
        public IActionResult CriarLivro(LivroDTOCreateEdit livroCreateEditDTO)
        {

            var result = _livroService.CriarLivro(livroCreateEditDTO);

            if (result.Sucesso)
                return CreatedAtAction(nameof(ObterLivroPorId), new { id = result.Dados!.LivroId }, result.Dados);

            return RespostaCustomizada(result);
        }
    }
}
