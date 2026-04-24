using AutoMapper;
using Biblioteca_WEB_API_REST_ASP.Models;
using static DTOS.Categoria.CategoriaDTO;

namespace Sistema.Application.Commoms.Mappings
{
    public class CategoriaProfile : Profile
    {
        public CategoriaProfile()
        {
            CreateMap<Categoria, CategoriaResponseDTO>();

            CreateMap<CategoriaUpdateDTO, Categoria>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Ativo, opt => opt.Ignore())
                .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
                .ForMember(dest => dest.DataUltimaAtualizacao, opt => opt.Ignore())
                .ForMember(dest => dest.IdUsuarioCriacao, opt => opt.Ignore());
        }
    }
}