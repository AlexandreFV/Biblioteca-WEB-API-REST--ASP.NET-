using Microsoft.AspNetCore.Mvc;
using Biblioteca_WEB_API_REST_ASP.Class;

namespace Biblioteca_WEB_API_REST_ASP.Class
{
    public class ControllerPersonalizado : ControllerBase
    {
        protected IActionResult RespostaCustomizada<T>(ServiceResult<T> resultado)
        {
            return resultado.Tipo switch
            {
                ResultType.Sucesso => Ok(resultado),
                ResultType.Criado => Created(string.Empty, resultado),
                ResultType.Atualizado => Ok(resultado),

                ResultType.NotFound => NotFound(resultado),
                ResultType.Conflito => Conflict(resultado),
                ResultType.Invalido => BadRequest(resultado),
                ResultType.NaoAutorizado => Unauthorized(resultado),
                ResultType.Erro => StatusCode(500, resultado),

                _ => BadRequest(resultado)
            };
        }
    }
}

