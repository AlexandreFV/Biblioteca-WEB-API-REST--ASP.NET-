using Biblioteca_WEB_API_REST_ASP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DTOS.Auth.AuthDTO;

namespace Sistema.Application.Interfaces.Services
{
    public interface IAuthService
    {

        Task<ServiceResult<RegisterDTOResponse>> CriarUsuarioAsync(RegisterDTOCreate registerDTOCreate, bool isAdmin);
        Task<ServiceResult<LoginResponseDTO>> EntrarAsync(LoginDTO loginDTO);
    }
}
