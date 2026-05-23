using AutoMapper;
using Biblioteca_WEB_API_REST_ASP.Models;
using static DTOS.Usuario.UsuarioDTO;

namespace Sistema.Application.Commoms.Mappings
{
    public class UsuarioProfile : Profile
    {

        public UsuarioProfile() 
        {
            CreateMap<Usuario, UsuarioDTOResponse>();
            CreateMap<Usuario, UsuarioDTOLogin>();
        }
    }
}
