using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Models;
using DTOS.SolicitacaoEmprestimo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DTOS.SolicitacaoEmprestimo.SolicitacaoEmprestimoDTO;

namespace Sistema.Application.Interfaces.Services
{
    public interface ISolicitacaoService : ICrudService<SolicitacaoEmprestimoDTOResponse, SolicitacaoEmprestimoDTOCreate, SolicitacaoEmprestimoDTOUpdateAdmin>
    {
        public Task<ServiceResult<SolicitacaoEmprestimoDTOResponse?>> alterarStatusDaSolicitacao(int idSolicitacao, SolicitacaoEmprestimoDTOUpdateAdmin alteracaoStatusEmprestimo);
    }
}
