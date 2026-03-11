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

        /// <summary>
        /// Listagem das solicitacoes
        /// </summary>
        /// <returns>Solicitacoes filtradas por permissao</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ServiceResult<IEnumerable<SolicitacaoEmprestimoDTOResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResult<IEnumerable<SolicitacaoEmprestimoDTOResponse>>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObterSolicitacaoAsync() => RespostaCustomizada(await _solicitacaoService.listarAsync());


        /// <summary>
        /// Listagem das solicitacoes do usuario logado
        /// </summary>
        /// <returns>Solicitacoes filtradas por id de usuario</returns>
        [HttpGet("minhas")]
        [Authorize(Roles = "Client")]
        [ProducesResponseType(typeof(ServiceResult<IEnumerable<SolicitacaoEmprestimoDTOResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResult<IEnumerable<SolicitacaoEmprestimoDTOResponse>>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ServiceResult<IEnumerable<SolicitacaoEmprestimoDTOResponse>>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ObterMinhasSolicitacoesAsync() => RespostaCustomizada(await _solicitacaoService.ListarMinhasSolicitacoesAsync());

        /// <summary>
        /// Detalhes da solicitacao especificado por Id
        /// </summary>
        /// <param name="id">Identificador da solicitacao</param>
        /// <returns>Solicitacao especificada com uso de permissao</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ServiceResult<SolicitacaoEmprestimoDTOResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResult<SolicitacaoEmprestimoDTOResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ServiceResult<SolicitacaoEmprestimoDTOResponse>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObterSolicitacaoPorId(int id) => RespostaCustomizada(await _solicitacaoService.buscarPorId(id));

        /// <summary>
        /// Adicao de nova solicitacao
        /// </summary>
        /// <param name="solicitacaoCreateDTO">Dados da nova solicitacao</param>
        /// <returns>Nova solicitacao adicionada em sistema</returns>
        [HttpPost]
        [Authorize(Roles = "Client")]
        [ProducesResponseType(typeof(SolicitacaoEmprestimoDTOResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ServiceResult<SolicitacaoEmprestimoDTOResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ServiceResult<SolicitacaoEmprestimoDTOResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ServiceResult<SolicitacaoEmprestimoDTOResponse>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ServiceResult<SolicitacaoEmprestimoDTOResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ServiceResult<SolicitacaoEmprestimoDTOResponse>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CriarSolicitacaoAsync(SolicitacaoEmprestimoDTOCreate solicitacaoCreateDTO)
        {

            var result = await _solicitacaoService.adicionarAsync(solicitacaoCreateDTO);

            if (result.Sucesso)
                return CreatedAtAction(nameof(ObterSolicitacaoPorId), new { id = result.Dados!.Id }, result.Dados);

            return RespostaCustomizada(result);
        }

        /// <summary>
        /// Edicao de status da solicitacao
        /// </summary>
        /// <param name="id">Identificador de solicitacao</param>
        /// <param name="solicitacaoDTO">Novo status da solicitacao</param>
        /// <returns>Solicitacao atualizada com novo status</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ServiceResult<SolicitacaoEmprestimoDTOResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResult<SolicitacaoEmprestimoDTOResponse>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ServiceResult<SolicitacaoEmprestimoDTOResponse>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ServiceResult<SolicitacaoEmprestimoDTOResponse>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ServiceResult<SolicitacaoEmprestimoDTOResponse>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ServiceResult<SolicitacaoEmprestimoDTOResponse>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> EditarStatusSolicitacaoAsync(int id, SolicitacaoEmprestimoDTOUpdateAdmin solicitacaoDTO) => RespostaCustomizada(await _solicitacaoService.alterarStatusDaSolicitacao(id, solicitacaoDTO));

        /// <summary>
        /// Alterar status de "ativo" da solicitacao
        /// </summary>
        /// <param name="id">Identificador de solicitacao</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ServiceResult<bool>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelarSolicitacaoAsync(int id) => RespostaCustomizada(await _solicitacaoService.softDeletePorId(id));

    }
}
