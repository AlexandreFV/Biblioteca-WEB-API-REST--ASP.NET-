using System.Text.Json;
using Biblioteca_WEB_API_REST_ASP.Class;

namespace Sistema.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _reqDele;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate reqDele, ILogger<ExceptionMiddleware> logger)
        {
            _reqDele = reqDele;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _reqDele(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erro não tratado ocorreu. Path: {Path}, Metodo: {Method}",
                    context.Request.Path,
                    context.Request.Method);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var result = new ServiceResult<object>
                {
                    Sucesso = false,
                    Mensagem = "Erro interno no servidor" + ex.ToString(),
                    Tipo = ResultType.Erro,
                    Dados = null
                };

                var json = JsonSerializer.Serialize(result);

                await context.Response.WriteAsync(json);
            }
        }
    }
}