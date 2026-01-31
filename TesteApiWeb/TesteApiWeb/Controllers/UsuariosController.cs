using DTOS.Usuario;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TesteApiWeb.Class;
using TesteApiWeb.Context;
using TesteApiWeb.Services;
using static DTOS.Usuario.UsuarioDTO;

namespace TesteApiWeb.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
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
