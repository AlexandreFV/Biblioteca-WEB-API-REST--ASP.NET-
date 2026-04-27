using AutoMapper;
using AutoMapper.QueryableExtensions;
using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Models;
using BibliotecaWebApiRest.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Sistema.Application.Commoms.Pagination;
using Sistema.Application.Interfaces;
using Sistema.Application.Interfaces.Services;
using Sistema.Application.Services;
using TesteApiWeb.Services;
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
        private readonly ILogger<SolicitacaoEmprestimoService> _logger;
        private readonly IMapper _mapper;
        public SolicitacaoEmprestimoService(ISolicitacaoEmprestimoRepository repo, ICurrentUser currentUser, ILivroRepository repoLivro, IIdentityService identityService, ILogger<SolicitacaoEmprestimoService> logger, IMapper mapper)
        {
            _repo = repo;
            _currentUser = currentUser;
            _repoLivro = repoLivro;
            _identityService = identityService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ServiceResult<SolicitacaoEmprestimoDTOResponse>> buscarPorId(int id)
        {

            _logger.LogInformation(
                "Usuário {UserId} solicitou a solicitacao de emprestimo com id: {id}. Admin: {IsAdmin}",
                _currentUser.userId,
                id,
                _currentUser.IsAdmin
            );

            var solicitacaoDesejada = await _repo.ObterSolicitacaoEmprestimoPorId(id, _currentUser.userId, _currentUser.IsAdmin, _currentUser.IsAdmin);

            if (solicitacaoDesejada == null || (!_currentUser.IsAdmin && (!solicitacaoDesejada.Ativo)))
            {
                _logger.LogWarning(
                    "Solicitacao de emprestimo com id: {id} não foi encontrada para o user {userId}",
                    id,
                    _currentUser.userId
                );

                return ServiceResult<SolicitacaoEmprestimoDTOResponse>.Error(
                        null,
                        "Não foi encontrado a solicitacao com ID: " + id,
                        ResultType.NotFound
                    );
            }

            _logger.LogInformation(
                "Solicitacao de emprestimo com id: {id} encontrada com sucesso para o user {userId}",
                id,
                _currentUser.userId
            );

            return ServiceResult<SolicitacaoEmprestimoDTOResponse>.Success(
                _mapper.Map<SolicitacaoEmprestimoDTOResponse>(solicitacaoDesejada),
                "Solicitação encontrada com ID: " + id,
                ResultType.Sucesso
            );

        }

        public async Task<ServiceResult<bool>> softDeletePorId(int id)
        {

            _logger.LogInformation(
                "Usuário {UserId} solicita a exclusão da solicitacao de emprestimo com id: {id}. Admin: {IsAdmin}",
                _currentUser.userId,
                id,
                _currentUser.IsAdmin
            );

            if (!_currentUser.IsAdmin)
            {
                _logger.LogWarning(
                    "Usuário {UserId} não possui permissão para excluir a solicitacao de emprestimo com id: {id}",
                    _currentUser.userId,
                    id
                );

                return ServiceResult<bool>.Error(
                    false,
                    "Apenas administradores podem apagar solicitações",
                    ResultType.NaoAutorizado
                );
            }

            _logger.LogInformation(
                "Iniciando procura da solicitacao de emprestimo com base no id {id}",
                id
            );

            var solicitacaoDesejada = await _repo.ObterSolicitacaoEmprestimoPorId(id, _currentUser.userId, _currentUser.IsAdmin, _currentUser.IsAdmin);

            if (solicitacaoDesejada == null)
            {
                _logger.LogWarning(
                    "Solicitacao de emprestimo: {id} não encontrado",
                    id
                );

                return ServiceResult<bool>.Error(
                        false,
                        "Não foi encontrada a solicitação desejada",
                        ResultType.NotFound
                    );
            }

            _logger.LogInformation(
                "Iniciando soft delete da Solicitacao de emprestimo com id: {id}",
                id
            );

            _repo.SoftDelete(solicitacaoDesejada);
            await _repo.SalvarAsync();

            _logger.LogInformation(
                "Soft delete da Solicitacao de emprestimo com id {id} realizado com sucesso",
                id
            );

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

            _logger.LogInformation(
                "Usuário {UserId} solicitou criação de nova Solicitacao de emprestimo. Admin: {IsAdmin}",
                _currentUser.userId,
                _currentUser.IsAdmin
            );


            if (solicitacaoCreateDTO == null)
            {

                _logger.LogWarning(
                    "Usuário {UserId} enviou DTO nulo para criação de Solicitacao de emprestimo",
                    _currentUser.userId
                );

                return ServiceResult<SolicitacaoEmprestimoDTOResponse>.Error(
                    null,
                    "É obrigatorio que o DTO Criar não seja nulo",
                    ResultType.Invalido
                );
            }

            _logger.LogInformation(
                "Verificando se o id {userId} de usuario é válido",
                _currentUser.userId
            );

            var usuarioIdSolicitacaoEmprestimo = await _identityService.encontrarUsuarioPorId(_currentUser.userId);

            if (usuarioIdSolicitacaoEmprestimo is null || usuarioIdSolicitacaoEmprestimo.Ativo == false)
            {

                _logger.LogWarning(
                    "O id {userId} de usuario é inválido",
                    _currentUser.userId
                );

                return ServiceResult<SolicitacaoEmprestimoDTOResponse>.Error(
                    null,
                    "Usuario vinculado não encontrado",
                    ResultType.NotFound
                );
            }

            _logger.LogInformation(
                "Verificando se o id {userId} do livro é válido",
                solicitacaoCreateDTO.IdLivro
            );

            var livroIdSolicitacaoEmprestimo = await _repoLivro.ObterLivroIncluindoCategoriaPorId(solicitacaoCreateDTO.IdLivro, false);

            if(livroIdSolicitacaoEmprestimo is null)
            {

                _logger.LogWarning(
                    "O id {userId} de livro é inválido",
                    solicitacaoCreateDTO.IdLivro
                );

                return ServiceResult<SolicitacaoEmprestimoDTOResponse>.Error(
                    null,
                    "Livro vinculado não encontrado",
                    ResultType.NotFound
                );
            }


            _logger.LogInformation(
                "Verificando se existe solicitacao duplicada para o usuario {userId} e livro {livroId}",
                _currentUser.userId,
                livroIdSolicitacaoEmprestimo.Id
            );

            var solicitacaoAnteriorMesmoLivro = await _repo.ExisteSolicitacaoAtivaParaUsuarioELivroAsync(_currentUser.userId, livroIdSolicitacaoEmprestimo.Id);
            
            if(solicitacaoAnteriorMesmoLivro is not false)
            {

                _logger.LogWarning(
                    "Já existe solicitacao para o usuario {userId} e livro {livroId}",
                    _currentUser.userId,
                    livroIdSolicitacaoEmprestimo.Id
                );

                return ServiceResult<SolicitacaoEmprestimoDTOResponse>.Error(
                    null,
                    "Já existe uma solicitacao ativa para o usuario com o livro escolhido",
                    ResultType.Conflito
                );
            }

            var solicitacaoEmprestimoEntity = new SolicitacaoEmprestimo
            (

                usuarioIdSolicitacaoEmprestimo.Id,
                livroIdSolicitacaoEmprestimo.Id
            )
            {
                DataSolicitacao = DateTime.UtcNow,
                DataUltimaAtualizacao = DateTime.UtcNow,
                DataAlteracaoStatus = DateTime.UtcNow,
                Ativo = true
            };


            await _repo.AdicionarAsync(solicitacaoEmprestimoEntity);
            await _repo.SalvarAsync();

            _logger.LogInformation(
                "Solicitacao de emprestimo {id} criada com sucesso pelo usuario {userId}",
                solicitacaoEmprestimoEntity.Id,
                _currentUser.userId
            );

            return ServiceResult<SolicitacaoEmprestimoDTOResponse>.Success(
                _mapper.Map<SolicitacaoEmprestimoDTOResponse>(solicitacaoEmprestimoEntity),
                "Solicitacao adicionada com sucesso",
                ResultType.Criado
            );
        }


        public async Task<ServiceResult<PagedResult<SolicitacaoEmprestimoDTOResponse>>> ListarMinhasSolicitacoesAsync(PaginationParams pagination)
        {
            _logger.LogInformation(
                "Usuário {UserId} solicitou listagem das próprias solicitações de emprestimo. Admin: {IsAdmin}",
                _currentUser.userId,
                _currentUser.IsAdmin
            );

            var solicitacao = _repo.GetQueryableComLivroUsuarioParaCliente(_currentUser.userId);

            var resultado = await solicitacao
                .ProjectTo<SolicitacaoEmprestimoDTOResponse>(_mapper.ConfigurationProvider)
                .ToPagedResultAsync(pagination.Page, pagination.PageSize);

            _logger.LogInformation(
                "Solicitacoes de emprestimo retornadas com sucesso para usuário {UserId}",
                _currentUser.userId
            );

            return ServiceResult<PagedResult<SolicitacaoEmprestimoDTOResponse>>.Success(
                resultado,
                "Solicitações encontradas com sucesso",
                ResultType.Sucesso
            );
        }

        public async Task<ServiceResult<SolicitacaoEmprestimoDTOResponse?>> alterarStatusDaSolicitacao(int idSolicitacao, SolicitacaoEmprestimoDTOUpdateAdmin alteracaoStatusEmprestimo)
        {

            _logger.LogInformation(
                "Usuário {UserId} solicitou alteração do Status da solicitação de emprestimo com id {solicitacaoId}. Admin: {IsAdmin}",
                _currentUser.userId,
                idSolicitacao,
                _currentUser.IsAdmin
            );

            if (!_currentUser.IsAdmin)
            {

                _logger.LogWarning(
                    "Usuario {userId} não tem permissão para alterar a solicitacao {solicitacaoId}",
                    _currentUser.userId,
                    idSolicitacao
                );

                return ServiceResult<SolicitacaoEmprestimoDTOResponse?>.Error(
                    null,
                    "Apenas administradores podem alterar o Status das solicitações",
                    ResultType.NaoAutorizado
                );
            }

            if (alteracaoStatusEmprestimo == null)
            {

                _logger.LogWarning(
                    "Usuario {userId} enviou um DTO invalido para alteração de status da solicitacao {solicitacaoId}",
                    _currentUser.userId,
                    idSolicitacao
                );

                return ServiceResult<SolicitacaoEmprestimoDTOResponse?>.Error(
                    null,
                    "É obrigatorio que o DTO Update não seja nulo",
                    ResultType.Invalido
                );
            }

            _logger.LogInformation(
                "Usuario {userId} está verificando se a solicitacao com id: {solicitaId} é válida e existente",
                _currentUser.userId,
                idSolicitacao
            );

            var solicitacaoDesejada = await _repo.ObterSolicitacaoEmprestimoPorId(idSolicitacao, _currentUser.userId, _currentUser.IsAdmin, _currentUser.IsAdmin);

            if (solicitacaoDesejada == null || !_currentUser.IsAdmin)
            {

                _logger.LogWarning(
                    "A solicitacao de emprestimo com id {solicitacaoId} que o usuario {userId} solicitou não foi encontrada",
                    idSolicitacao,
                    _currentUser.userId
                );

                return ServiceResult<SolicitacaoEmprestimoDTOResponse?>.Error(
                    null,
                    "Não foi encontrada a solicitação desejada",
                    ResultType.NotFound
                );
            }

            if (solicitacaoDesejada.Status == StatusSolicitacao.Aprovado)
            {
                _logger.LogWarning(
                    "A solicitacao de emprestimo com id {solicitacaoId} que o usuario {userId} solicitou, já está aprovada. Não é possivel realizar alteração de status",
                    idSolicitacao,
                    _currentUser.userId
                );

                return ServiceResult<SolicitacaoEmprestimoDTOResponse?>.Error(
                    null,
                    "Não é possivel alterar o Status de uma solicitacao com Status Aprovado",
                    ResultType.Conflito
                );
            }

            solicitacaoDesejada.Status = alteracaoStatusEmprestimo.Status;
            solicitacaoDesejada.DataAlteracaoStatus = DateTime.UtcNow;
            solicitacaoDesejada.IdUsuarioAdmin = _currentUser.userId;
            
            await _repo.SalvarAsync();

            _logger.LogInformation(
                "Realizada alteração de status para a solicitacao de emprestimo com id {solicitaId} pelo usuario {userId}",
                idSolicitacao,
                _currentUser.userId
            );

            return ServiceResult<SolicitacaoEmprestimoDTOResponse?>.Success(
                _mapper.Map<SolicitacaoEmprestimoDTOResponse>(solicitacaoDesejada),
                "Status da Solicitação alterado com sucesso",
                ResultType.Atualizado
            );

        }

        public async Task<ServiceResult<PagedResult<SolicitacaoEmprestimoDTOResponse>>> listarAsync(PaginationParams pagination)
        {

            _logger.LogInformation(
                "Usuário {UserId} solicitou listagem das solicitações de emprestimo. Admin: {IsAdmin}",
                _currentUser.userId,
                _currentUser.IsAdmin
            );

            var solicitacao = _repo.GetQueryableComLivroUsuarioParaAdmin();

            var resultado = await solicitacao
                .ProjectTo<SolicitacaoEmprestimoDTOResponse>(_mapper.ConfigurationProvider)
                .ToPagedResultAsync(pagination.Page, pagination.PageSize);

            _logger.LogInformation(
                "Solicitacoes de emprestimo retornadas com sucesso para usuário {UserId}",
                _currentUser.userId
            );

            return ServiceResult<PagedResult<SolicitacaoEmprestimoDTOResponse>>.Success(
                resultado,
                "Solicitações encontradas com sucesso",
                ResultType.Sucesso
            );
        }
    }
}
