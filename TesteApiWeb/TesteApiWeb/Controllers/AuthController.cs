using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TesteApiWeb.Class;
using TesteApiWeb.Models;
using TesteApiWeb.Services;
using static TesteApiWeb.Models.AuthDTO;

namespace TesteApiWeb.Controllers
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
        [HttpPost("Entrar")]
        public async Task<IActionResult> EntrarAsync(LoginDTO loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);
            return RespostaCustomizada(result);
        }

        [AllowAnonymous]
        [HttpPost("Registrar")]
        public async Task<IActionResult> RegistrarAsync(RegisterDTOCreate registerDTO) 
        { 
            var result = await _authService.CriarUsuarioAsync(registerDTO);
            return RespostaCustomizada(result);

        }
    }
}
