using Microsoft.AspNetCore.Mvc;

namespace TesteApiWeb.Class
{
    public class ControllerPersonalizado : ControllerBase
    {
        protected IActionResult RespostaCustomizada<T>(ServiceResult<T> resultado)
        {
            if (resultado.Sucesso)
            {
                return Ok(new
                {
                    mensagem = resultado.Mensagem,
                    dados = resultado.Dados
                });
            }

            return resultado.Tipo switch
            {
                ResultType.NotFound => NotFound(new { erro = resultado.Mensagem }),
                ResultType.Conflito => Conflict(new { erro = resultado.Mensagem }),
                ResultType.Invalido => BadRequest(new { erro = resultado.Mensagem }),
                ResultType.Erro => StatusCode(500, new { erro = resultado.Mensagem }),
                _ => BadRequest(new { erro = resultado.Mensagem })
            };
        }
    }
}

