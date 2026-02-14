using Biblioteca_WEB_API_REST_ASP;
using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static DTOS.Auth.AuthDTO;

namespace Biblioteca_WEB_API_REST_ASP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerPersonalizado
    {

        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService; 
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> EntrarAsync(LoginDTO loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            return RespostaCustomizada(result);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegistrarAsync(RegisterDTOCreate registerDTO, bool isAdmin) 
        { 
            var result = await _authService.CriarUsuarioAsync(registerDTO, isAdmin);
            return RespostaCustomizada(result);

        }
    }
}
