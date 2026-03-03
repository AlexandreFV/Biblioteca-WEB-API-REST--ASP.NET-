using Biblioteca_WEB_API_REST_ASP;
using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sistema.Application.Interfaces.Services;
using System.Security.Claims;
using static DTOS.Auth.AuthDTO;

namespace Biblioteca_WEB_API_REST_ASP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerPersonalizado
    {

        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService; 
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> EntrarAsync(LoginDTO loginDto)
        {
            var result = await _authService.EntrarAsync(loginDto);
            return RespostaCustomizada(result);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> RegistrarAsync(RegisterDTOCreate registerDTO, bool isAdmin) 
        { 
            var result = await _authService.CriarUsuarioAsync(registerDTO, isAdmin);
            return RespostaCustomizada(result);

        }

        [HttpGet("debug")]
        public IActionResult DebugToken()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();
            return Ok(new { userId, roles });
        }

    }
}
