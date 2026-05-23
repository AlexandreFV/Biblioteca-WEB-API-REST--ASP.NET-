using AutoMapper;
using Biblioteca_WEB_API_REST_ASP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DTOS.Livro.LivroDTO;

namespace Sistema.Application.Commoms.Mappings
{
    public class LivroProfile : Profile
    {

        public LivroProfile() 
        {
            CreateMap<Livro, LivroResponseDTO>();

        }
    }
}
