using Biblioteca_WEB_API_REST_ASP;
using Biblioteca_WEB_API_REST_ASP.Class;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TesteApiWeb.Services;
using static DTOS.Usuario.UsuarioDTO;

namespace Biblioteca_WEB_API_REST_ASP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("porUsuario")]
    public class UsuariosController : ControllerPersonalizado
    {

        private readonly UsuarioService _usuarioService;

        public UsuariosController(UsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpGet]
        public async Task<IActionResult> ObterUsuariosAsync() => RespostaCustomizada(await _usuarioService.ListarUsuariosAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterUsuarioPorIdAsync(string id) => RespostaCustomizada(await _usuarioService.ProcurarUsuarioAsync(id));

        [HttpPut("{id}")]
        public async Task<IActionResult> EditarUsuarioAsync(string id, UsuarioDTOEdit usuarioDTO) => RespostaCustomizada(await _usuarioService.EditarUsuarioAsync(id, usuarioDTO));

        [HttpDelete("{id}")]
        public async Task<IActionResult> ExcluirUsuarioAsync(string id) => RespostaCustomizada(await _usuarioService.ApagarUsuarioAsync(id));
    }
}
