using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TesteApiWeb.Class;
using TesteApiWeb.Context;
using TesteApiWeb.Models;
using TesteApiWeb.Services;

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
        public IActionResult ObterUsuarios() => RespostaCustomizada(_usuarioService.ListarUsuarios());

        [HttpGet("{id}")]
        public IActionResult ObterUsuarioPorId(int id) => RespostaCustomizada(_usuarioService.ProcurarUsuario(id));

        [HttpPost]
        public IActionResult AdicionarUsuario(UsuarioDTO usuarioDTO)
        {
            var result = _usuarioService.CriarUsuario(usuarioDTO);

            if (result.Sucesso)
                return CreatedAtAction(nameof(ObterUsuarioPorId), new { id = result.Dados!.UsuarioId }, result.Dados);

            return RespostaCustomizada(result);
        }

        [HttpPut("{id}")]
        public IActionResult EditarUsuario(int id, UsuarioDTO usuarioDTO) => RespostaCustomizada(_usuarioService.EditarUsuario(id, usuarioDTO));

        [HttpDelete("{id}")]
        public IActionResult ExcluirUsuario(int id) => RespostaCustomizada(_usuarioService.ApagarUsuario(id));
    }
}
