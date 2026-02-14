using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TesteApiWeb.Services;
using static DTOS.Livro.LivroDTO;
using static DTOS.SolicitacaoEmprestimo.SolicitacaoEmprestimoDTO;

namespace BibliotecaWebApiRest.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SolicitacaoController : ControllerPersonalizado
    {
        private readonly SolicitacaoEmprestimoService _solicitacaoService;

        public SolicitacaoController(SolicitacaoEmprestimoService solicitacaoEmprestimoService)
        {
            _solicitacaoService = solicitacaoEmprestimoService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterSolicitacaoAsync() => RespostaCustomizada(await _solicitacaoService.ListarSolicitacoesEmprestimosAsync(User));


        [HttpGet("{id}")]
        public async Task<IActionResult> ObterSolicitacaoPorId(int id) => RespostaCustomizada(await _solicitacaoService.ListarSolicitacaoEmprestimoPorIdAsync(id, User));

        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> CriarSolicitacaoAsync(SolicitacaoEmprestimoDTOCreate solicitacaoCreateDTO)
        {

            var result = await _solicitacaoService.CriarSolicitacaoEmprestimoAsync(solicitacaoCreateDTO, User);

            if (result.Sucesso)
                return CreatedAtAction(nameof(ObterSolicitacaoPorId), new { id = result.Dados!.Id }, result.Dados);

            return RespostaCustomizada(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditarStatusSolicitacaoAsync(int id, SolicitacaoEmprestimoDTOUpdateAdmin solicitacaoDTO) => RespostaCustomizada(await _solicitacaoService.AlterarStatusSolicitacaoAsync(id, solicitacaoDTO));

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CancelarSolicitacaoAsync(int id) => RespostaCustomizada(await _solicitacaoService.CancelarSolicitacaoEmprestimoAsync(id));

    }
}
