namespace TesteApiWeb.Class
{
    public class ServiceResult<T>
    {

        public bool Sucesso { get; set; }
        public String Mensagem { get; set; } = String.Empty;
        public T? Dados { get; set; }
        public ResultType Tipo { get; set; }

        public static ServiceResult<T> Success(T dados) =>
            new() { Sucesso = true, Dados = dados, Tipo = ResultType.Sucesso };

        public static ServiceResult<T> Error(ResultType tipo, string mensagem) =>
            new() { Sucesso = false, Mensagem = mensagem, Tipo = tipo };
    }

    public enum ResultType { Sucesso, Criado, Atualizado, NotFound, Invalido, Conflito }
}
