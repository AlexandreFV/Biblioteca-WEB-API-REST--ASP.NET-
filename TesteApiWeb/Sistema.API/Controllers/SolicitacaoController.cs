using Biblioteca_WEB_API_REST_ASP.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema.Application.Interfaces.Services;
using static DTOS.SolicitacaoEmprestimo.SolicitacaoEmprestimoDTO;

namespace BibliotecaWebApiRest.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SolicitacaoController : ControllerPersonalizado
    {
        private readonly ISolicitacaoService _solicitacaoService;

        public SolicitacaoController(ISolicitacaoService solicitacaoEmprestimoService)
        {
            _solicitacaoService = solicitacaoEmprestimoService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterSolicitacaoAsync() => RespostaCustomizada(await _solicitacaoService.listarAsync());


        [HttpGet("{id}")]
        public async Task<IActionResult> ObterSolicitacaoPorId(int id) => RespostaCustomizada(await _solicitacaoService.buscarPorId(id));

        [HttpPost]
        [Authorize(Roles = "Client")]
        public async Task<IActionResult> CriarSolicitacaoAsync(SolicitacaoEmprestimoDTOCreate solicitacaoCreateDTO)
        {

            var result = await _solicitacaoService.adicionarAsync(solicitacaoCreateDTO);

            if (result.Sucesso)
                return CreatedAtAction(nameof(ObterSolicitacaoPorId), new { id = result.Dados!.Id }, result.Dados);

            return RespostaCustomizada(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditarStatusSolicitacaoAsync(int id, SolicitacaoEmprestimoDTOUpdateAdmin solicitacaoDTO) => RespostaCustomizada(await _solicitacaoService.alterarStatusDaSolicitacao(id, solicitacaoDTO));


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CancelarSolicitacaoAsync(int id) => RespostaCustomizada(await _solicitacaoService.softDeletePorId(id));

    }
}
