using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Models;
using BibliotecaWebApiRest.Repositories.Interfaces;
using Sistema.Application.Interfaces;
using Sistema.Application.Interfaces.Services;
using Sistema.Application.Services;
using static DTOS.Livro.LivroDTO;
using static DTOS.SolicitacaoEmprestimo.SolicitacaoEmprestimoDTO;
using static DTOS.Usuario.UsuarioDTO;

namespace Biblioteca_WEB_API_REST_ASP.Services
{
    public class SolicitacaoEmprestimoService : PadronizarTextoService, ISolicitacaoService
    {

        private readonly ISolicitacaoEmprestimoRepository _repo;
        private readonly ICurrentUser _currentUser;
        private readonly ILivroRepository _repoLivro;
        private readonly IIdentityService _identityService;

        public SolicitacaoEmprestimoService(ISolicitacaoEmprestimoRepository repo, ICurrentUser currentUser, ILivroRepository repoLivro, IIdentityService identityService)
        {
            _repo = repo;
            _currentUser = currentUser;
            _repoLivro = repoLivro;
            _identityService = identityService;
        }

        private SolicitacaoEmprestimoDTOResponse Mapear(SolicitacaoEmprestimo s)
        {
            var solicitacaoMapeada = new SolicitacaoEmprestimoDTOResponse
            {
                Id = s.Id,
                IdUsuarioCliente = s.IdUsuarioCliente,
                IdLivro = s.IdLivro,
                DataSolicitacao = s.DataSolicitacao,
                Status = s.Status,
                UsuarioCliente = s.UsuarioCliente == null ? null : new UsuarioDTOResponse
                {
                    UsuarioId = s.UsuarioCliente.Id,
                    Nome = s.UsuarioCliente.Nome,
                    Ativo = s.UsuarioCliente.Ativo,
                },
                UsuarioAdmin = s.UsuarioAdmin == null ? null : new UsuarioDTOResponse
                {
                    UsuarioId = s.UsuarioAdmin.Id,
                    Nome = s.UsuarioAdmin.Nome,
                    Ativo = s.UsuarioAdmin.Ativo,
                },
                LivroSolicitado = s.LivroSolicitado == null ? null : new LivroResponseDTO
                {
                    Id = s.LivroSolicitado.Id,
                    Nome = s.LivroSolicitado.Nome,
                }
            };

            return solicitacaoMapeada;
        }


        public async Task<ServiceResult<SolicitacaoEmprestimoDTOResponse>> buscarPorId(int id)
        {
            var solicitacaoDesejada = await _repo.ObterSolicitacaoEmprestimoPorId(id, _currentUser.userId, _currentUser.IsAdmin, _currentUser.IsAdmin);

            if (solicitacaoDesejada == null || (!_currentUser.IsAdmin && (!solicitacaoDesejada.Ativo)))
                return ServiceResult<SolicitacaoEmprestimoDTOResponse>.Error(
                    null,
                    "Não foi encontrado a solicitacao com ID: " + id,
                    ResultType.NotFound
                );

            return ServiceResult<SolicitacaoEmprestimoDTOResponse>.Success(
                Mapear(solicitacaoDesejada),
                "Solicitação encontrada com ID: " + id,
                ResultType.Sucesso
            );

        }

        public async Task<ServiceResult<bool>> softDeletePorId(int id)
        {
            if (!_currentUser.IsAdmin)
                return ServiceResult<bool>.Error(
                    false,
                    "Apenas administradores podem apagar solicitações",
                    ResultType.NaoAutorizado
                );

            var solicitacaoDesejada = await _repo.ObterSolicitacaoEmprestimoPorId(id, _currentUser.userId, _currentUser.IsAdmin, _currentUser.IsAdmin);

            if (solicitacaoDesejada == null)
                return ServiceResult<bool>.Error(
                    false,
                    "Não foi encontrada a solicitação desejada",
                    ResultType.NotFound
                );

            _repo.SoftDelete(solicitacaoDesejada);
            await _repo.SalvarAsync();

            return ServiceResult<bool>.Success(
                true,
                "Solicitação Excluida com sucesso",
                ResultType.Sucesso
            );

        }

        public Task<ServiceResult<bool>> editarAsync(int id, SolicitacaoEmprestimoDTOUpdateAdmin TDtoEdit)
        {
            //Rota não será utilizada
            throw new NotImplementedException();
        }

        public async Task<ServiceResult<SolicitacaoEmprestimoDTOResponse>> adicionarAsync(SolicitacaoEmprestimoDTOCreate solicitacaoCreateDTO)
        {
            if (solicitacaoCreateDTO == null)
                return ServiceResult<SolicitacaoEmprestimoDTOResponse>.Error(
                    null,
                    "É obrigatorio que o DTO Criar não seja nulo",
                    ResultType.Invalido
                );

            var usuarioIdSolicitacaoEmprestimo = await _identityService.encontrarUsuarioPorId(_currentUser.userId);

            if (usuarioIdSolicitacaoEmprestimo is null || usuarioIdSolicitacaoEmprestimo.Ativo == false)
                return ServiceResult<SolicitacaoEmprestimoDTOResponse>.Error(
                    null,
                    "Usuario vinculado não encontrado",
                    ResultType.NotFound
                );

            var livroIdSolicitacaoEmprestimo = await _repoLivro.ObterLivroIncluindoCategoriaPorId(solicitacaoCreateDTO.IdLivro, false);

            if(livroIdSolicitacaoEmprestimo is null)
                return ServiceResult<SolicitacaoEmprestimoDTOResponse>.Error(
                    null,
                    "Livro vinculado não encontrado",
                    ResultType.NotFound
                );


            var solicitacaoAnteriorMesmoLivro = await _repo.ExisteSolicitacaoAtivaParaUsuarioELivroAsync(_currentUser.userId, livroIdSolicitacaoEmprestimo.Id);
            
            if(solicitacaoAnteriorMesmoLivro is not false)
                return ServiceResult<SolicitacaoEmprestimoDTOResponse>.Error(
                    null,
                    "Já existe uma solicitacao ativa para o usuario com o livro escolhido",
                    ResultType.Conflito
                );

            var solicitacaoEmprestimoEntity = new SolicitacaoEmprestimo
            {
                IdLivro = livroIdSolicitacaoEmprestimo.Id,
                IdUsuarioCliente = usuarioIdSolicitacaoEmprestimo.Id,
                Status = StatusSolicitacao.Aguardando,
                DataSolicitacao = DateTime.UtcNow,
                Ativo = true,
            };

            await _repo.AdicionarAsync(solicitacaoEmprestimoEntity);
            await _repo.SalvarAsync();

            return ServiceResult<SolicitacaoEmprestimoDTOResponse>.Success(
                Mapear(solicitacaoEmprestimoEntity),
                "Solicitacao adicionada com sucesso",
                ResultType.Criado
            );
        }


        public async Task<ServiceResult<IEnumerable<SolicitacaoEmprestimoDTOResponse>>> listarAsync()
        {
            var solicitacoesBanco = await _repo.ObterTodosPorPermissaoAsync(_currentUser.IsAdmin, _currentUser.IsAdmin, _currentUser.userId);

            return ServiceResult<IEnumerable<SolicitacaoEmprestimoDTOResponse>>.SuccessList(
                solicitacoesBanco.Select(Mapear),
                "Solicitações encontradas com sucesso",
                ResultType.Sucesso
            );
        }

        public async Task<ServiceResult<IEnumerable<SolicitacaoEmprestimoDTOResponse>>> ListarMinhasSolicitacoesAsync()
        {
            var solicitacoesBanco = await _repo.ObterTodasAsMinhasSolicitacoes(_currentUser.userId);

            return ServiceResult<IEnumerable<SolicitacaoEmprestimoDTOResponse>>.SuccessList(
                solicitacoesBanco.Select(Mapear),
                "Solicitações encontradas com sucesso",
                ResultType.Sucesso
            );
        }

        public async Task<ServiceResult<SolicitacaoEmprestimoDTOResponse?>> alterarStatusDaSolicitacao(int idSolicitacao, SolicitacaoEmprestimoDTOUpdateAdmin alteracaoStatusEmprestimo)
        {

            if (!_currentUser.IsAdmin)
                return ServiceResult<SolicitacaoEmprestimoDTOResponse?>.Error(
                    null,
                    "Apenas administradores podem alterar o Status das solicitações",
                    ResultType.NaoAutorizado
                );

            if (alteracaoStatusEmprestimo == null)
                return ServiceResult<SolicitacaoEmprestimoDTOResponse?>.Error(
                    null,
                    "É obrigatorio que o DTO Update não seja nulo",
                    ResultType.Invalido
                );

            var solicitacaoDesejada = await _repo.ObterSolicitacaoEmprestimoPorId(idSolicitacao, _currentUser.userId, _currentUser.IsAdmin, _currentUser.IsAdmin);

            if (solicitacaoDesejada == null || !_currentUser.IsAdmin)
                return ServiceResult<SolicitacaoEmprestimoDTOResponse?>.Error(
                    null,
                    "Não foi encontrada a solicitação desejada",
                    ResultType.NotFound
                );

            if (solicitacaoDesejada.Status == StatusSolicitacao.Aprovado)
                return ServiceResult<SolicitacaoEmprestimoDTOResponse?>.Error(
                    null,
                    "Não é possivel alterar o Status de uma solicitacao com Status Aprovado",
                    ResultType.Conflito
                );

            solicitacaoDesejada.Status = alteracaoStatusEmprestimo.Status;
            solicitacaoDesejada.DataAlteracaoStatus = DateTime.UtcNow;
            solicitacaoDesejada.IdUsuarioAdmin = _currentUser.userId;
            
            await _repo.SalvarAsync();

            return ServiceResult<SolicitacaoEmprestimoDTOResponse?>.Success(
                Mapear(solicitacaoDesejada),
                "Status da Solicitação alterado com sucesso",
                ResultType.Atualizado
            );

        }
    }
}
