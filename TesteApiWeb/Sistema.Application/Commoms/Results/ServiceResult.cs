namespace Biblioteca_WEB_API_REST_ASP.Class
{
    public class ServiceResult<T>
    {

        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = String.Empty;
        public T? Dados { get; set; }
        public ResultType Tipo { get; set; }
        
        public static ServiceResult<T> Success(T dados, string mensagem, ResultType StatusCodeEnum) =>
            new() { Sucesso = true, Dados = dados, Mensagem = mensagem,  Tipo = StatusCodeEnum };

        public static ServiceResult<IEnumerable<T>> SuccessList<T>(IEnumerable<T> dados, string mensagem, ResultType StatusCodeEnum) =>
            new()
            {
                Sucesso = true,
                Dados = dados,
                Mensagem = mensagem,
                Tipo = StatusCodeEnum
            };

        public static ServiceResult<T> Error(T? dados, string mensagem, ResultType StatusCodeEnum) =>
            new() { Sucesso = false, Dados = dados, Mensagem = mensagem, Tipo = StatusCodeEnum };
    }

    public enum ResultType { Sucesso, Criado, Atualizado, NotFound, Invalido, Conflito, NaoAutorizado, Erro }
}
