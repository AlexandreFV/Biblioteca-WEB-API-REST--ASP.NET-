namespace TesteApiWeb.Class
{
    public abstract class ServicePersonalizado<T> where T : class
    {

        protected readonly string NomeEntidade = typeof(T).Name;

        protected string JaExisteEsseNome => $"Já possui um(a) {NomeEntidade} com esse nome";
        protected string AdicionadoSucesso => $"{NomeEntidade} adicionado(a) com sucesso";
        protected string EncontradasSucesso => $"{NomeEntidade}s localizados(as) com sucesso";
        protected string NaoEncontrado => $"{NomeEntidade} não encontrado(a)";
        protected string AtualizadoSucesso => $"{NomeEntidade} atualizado(a) com sucesso";
        protected string IdDiferente => $"O ID do(a) {NomeEntidade} no corpo é diferente do ID da URL";
        protected string RegistrosVinculados => $"Não é possível excluir: existem registros vinculados a esta {NomeEntidade}.";
        protected string ExcluidoSucesso => $"{NomeEntidade} excluido com sucesso.";
        protected string JaExisteEsseEmail => $"Já possui um(a) {NomeEntidade} com esse e-mail";
        protected string UsuarioOuSenhaInvalidos => $"O Usuario ou senha está incorreto!";
        protected string ErroAoSalvar => $"Não foi possivel salvar o(a) {NomeEntidade}!";
        protected string ErroAoEditar => $"Não foi possivel editar o(a) {NomeEntidade}!";
        protected string ErroAoApagar => $"Não foi possivel apagar o(a) {NomeEntidade}!";

        protected string PadronizarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome)) return string.Empty;

            return System.Globalization.CultureInfo.CurrentCulture.TextInfo
                .ToTitleCase(nome.Trim().ToLower());
        }

        protected ServiceResult<TResult> Result<TResult>(bool sucess, string message, TResult? dados, ResultType tipo)
        {
            return new ServiceResult<TResult>
            {
                Sucesso = sucess,
                Mensagem = message,
                Dados = dados,
                Tipo = tipo
            };
        }

    }
}
