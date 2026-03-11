using Biblioteca_WEB_API_REST_ASP.Class;
using static DTOS.SolicitacaoEmprestimo.SolicitacaoEmprestimoDTO;

namespace Sistema.Application.Interfaces.Services
{
    public interface ISolicitacaoService : ICrudService<SolicitacaoEmprestimoDTOResponse, SolicitacaoEmprestimoDTOCreate, SolicitacaoEmprestimoDTOUpdateAdmin>
    {
        public Task<ServiceResult<SolicitacaoEmprestimoDTOResponse?>> alterarStatusDaSolicitacao(int idSolicitacao, SolicitacaoEmprestimoDTOUpdateAdmin alteracaoStatusEmprestimo);
        public Task<ServiceResult<IEnumerable<SolicitacaoEmprestimoDTOResponse>>> ListarMinhasSolicitacoesAsync();
    }
}
