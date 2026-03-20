using Biblioteca_WEB_API_REST_ASP.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Sistema.Application.Interfaces.Services;
using static DTOS.Auth.AuthDTO;

namespace Biblioteca_WEB_API_REST_ASP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableRateLimiting("limiteRequestRegisterLogin")]
    public class AuthController : ControllerPersonalizado
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService; 
        }

        /// <summary>
        /// Realiza o login do usuario
        /// </summary>
        /// <param name="loginDto">Dados de usuario para login</param>
        /// <returns>Token JWT</returns>
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(typeof(ServiceResult<LoginResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResult<LoginResponseDTO>), StatusCodes.Status401Unauthorized)]

        public async Task<IActionResult> EntrarAsync(LoginDTO loginDto)
        {
            var result = await _authService.EntrarAsync(loginDto);
            return RespostaCustomizada(result);
        }

        /// <summary>
        /// Cria um novo usuario
        /// </summary>
        /// <param name="registerDTO">Dados de usuario para registro</param>
        /// <param name="isAdmin">Permissao de usuario</param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(typeof(ServiceResult<RegisterDTOResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ServiceResult<RegisterDTOResponse>), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ServiceResult<RegisterDTOResponse>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RegistrarAsync(RegisterDTOCreate registerDTO, bool isAdmin) 
        { 
            var result = await _authService.CriarUsuarioAsync(registerDTO, isAdmin);
            return RespostaCustomizada(result);

        }
    }
}
