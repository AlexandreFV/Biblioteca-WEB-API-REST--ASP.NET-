using System.Text.Json;
using System.Threading.Tasks;

namespace Sistema.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _reqDele;

        public ExceptionMiddleware(RequestDelegate reqDele) { _reqDele = reqDele;  }

        public async Task Invoke(HttpContext context)
        {

            try
            {
                await _reqDele(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(
                    new
                    {
                        message = "Erro interno no servidor",
                        detail =   ex.Message,
                    }
                );

                await context.Response.WriteAsync(result);

            }
        }
    }
}
