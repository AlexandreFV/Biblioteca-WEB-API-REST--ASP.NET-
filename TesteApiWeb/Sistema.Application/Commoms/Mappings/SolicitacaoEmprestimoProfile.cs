using AutoMapper;
using Biblioteca_WEB_API_REST_ASP.Models;
using static DTOS.SolicitacaoEmprestimo.SolicitacaoEmprestimoDTO;

namespace Sistema.Application.Commoms.Mappings
{
    public class SolicitacaoEmprestimoProfile : Profile
    {
        public SolicitacaoEmprestimoProfile()
        {
            CreateMap<SolicitacaoEmprestimo, SolicitacaoEmprestimoDTOResponse>();
        }
    }
}
