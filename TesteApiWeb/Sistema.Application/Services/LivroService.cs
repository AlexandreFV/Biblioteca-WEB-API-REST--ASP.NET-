using AutoMapper;
using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Models;
using BibliotecaWebApiRest.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Sistema.Application.Commoms.Pagination;
using Sistema.Application.Interfaces;
using Sistema.Application.Interfaces.Services;
using Sistema.Application.Services;
using static DTOS.Livro.LivroDTO;
using AutoMapper.QueryableExtensions;

namespace TesteApiWeb.Services
{
    public class LivroService : PadronizarTextoService, ILivroService
    {

        private readonly ILivroRepository _repo;
        private readonly ICategoriaRepository _categoriaRepo;
        private readonly ICurrentUser _currentUser;
        private readonly IMapper _mapper;
        private readonly ILogger<LivroService> _logger;

        public LivroService(ILivroRepository livroRepository, ICategoriaRepository categoriaRepository, ICurrentUser currentUser, IMapper mapper, ILogger<LivroService> logger) { _repo = livroRepository; _categoriaRepo = categoriaRepository; _currentUser = currentUser; _mapper = mapper; _logger = logger; }

        public async Task<ServiceResult<PagedResult<LivroResponseDTO>>> listarAsync(PaginationParams pagination)
        {

            _logger.LogInformation(
                "Usuário {UserId} solicitou listagem dos livros. Admin: {IsAdmin}",
                _currentUser.userId,
                _currentUser.IsAdmin
            );

            var livros = _repo.GetQueryableComCategoria(_currentUser.IsAdmin);

            var resultado = await livros
                .ProjectTo<LivroResponseDTO>(_mapper.ConfigurationProvider)
                .ToPagedResultAsync(pagination.Page, pagination.PageSize);


            _logger.LogInformation(
                "Livros retornadas com sucesso para usuário {UserId}",
                _currentUser.userId
            );

            return ServiceResult<PagedResult<LivroResponseDTO>>.Success(
                resultado,
                "Livros localizadas com sucesso",
                ResultType.Sucesso
            );
        }

        public async Task<ServiceResult<LivroResponseDTO>> buscarPorId(int id)
        {

            _logger.LogInformation(
                "Usuário {UserId} solicitou um livro com id: {id}. Admin: {IsAdmin}",
                _currentUser.userId,
                id,
                _currentUser.IsAdmin
            );

            var livro = await _repo.ObterLivroIncluindoCategoriaPorId(id, _currentUser.IsAdmin);

            if (livro == null || (!_currentUser.IsAdmin && (!livro.Ativo || livro.Quantidade <= 0)))

            {
                _logger.LogWarning(
                    "Livro com id: {id} não foi encontrada para o user {userId}",
                    id,
                    _currentUser.userId
                );

                return ServiceResult<LivroResponseDTO>.Error(
                    null,
                    "Não foi encontrado o livro com ID: " + id,
                    ResultType.NotFound
                );
            }

            _logger.LogInformation(
                "Livro com id: {id} encontrada com sucesso para o user {userId}",
                id,
                _currentUser.userId
            );

            return ServiceResult<LivroResponseDTO>.Success(
                _mapper.Map<LivroResponseDTO>(livro),
                "Livro encontrado com ID: " + id,
                ResultType.Sucesso
            );
        }

        public async Task<ServiceResult<bool>> softDeletePorId(int id)
        {

            _logger.LogInformation(
                "Usuário {UserId} solicita a exclusão do livro com id: {id}. Admin: {IsAdmin}",
                _currentUser.userId,
                id,
                _currentUser.IsAdmin
            );

            if (!_currentUser.IsAdmin)
            {
                _logger.LogWarning(
                    "Usuário {UserId} não possui permissão para excluir o livro com id: {id}",
                    _currentUser.userId,
                    id
                );

                return ServiceResult<bool>.Error(
                        false,
                        "Apenas administradores podem apagar livros",
                        ResultType.NaoAutorizado
                    );
            }

            _logger.LogInformation(
                "Iniciando procura do livro com base no id {id}",
                id
            );

            var livro = await _repo.ObterPorIdAsync(id, _currentUser.IsAdmin);

            if (livro == null)
            {
                _logger.LogWarning(
                    "Livro: {id} não encontrado",
                    id
                );

                return ServiceResult<bool>.Error(
                    false,
                    "Não foi encontrado o livro com ID: " + id,
                    ResultType.NotFound
                );

            }

            _logger.LogInformation(
                "Iniciando soft delete do livro {id}",
                id
            );

            _repo.SoftDelete(livro);
            await _repo.SalvarAsync();

            _logger.LogInformation(
                "Soft delete do Livro {id} realizado com sucesso",
                id
            );

            return ServiceResult<bool>.Success(
                true,
                "Livro apagado com sucesso",
                ResultType.Sucesso
            );
        }

        public async Task<ServiceResult<bool>> editarAsync(int id, LivroEditDTO livroEditDTO)
        {

            _logger.LogInformation(
                "Usuário {UserId} solicitou edição do livro {livroId}. Admin: {IsAdmin}",
                _currentUser.userId,
                id,
                _currentUser.IsAdmin
            );


            if (!_currentUser.IsAdmin)
            {
                _logger.LogWarning(
                    "Usuário {UserId} tentou editar o livro {id} sem permissão",
                    _currentUser.userId,
                    id
                );

                return ServiceResult<bool>.Error(
                        false,
                        "Apenas administradores podem editar livros",
                        ResultType.NaoAutorizado
                    );
            }

            if (livroEditDTO == null)
            {
                _logger.LogWarning(
                    "Usuário {UserId} enviou DTO nulo para edição do livro {id}",
                    _currentUser.userId,
                    id
                );

                return ServiceResult<bool>.Error(
                        false,
                        "É obrigatorio que o DTO Editar não seja nulo",
                        ResultType.Invalido
                    );
            }

            _logger.LogInformation(
                "Buscando livro {id} para edição",
                id
            );

            var livro = await _repo.ObterPorIdAsync(id, _currentUser.IsAdmin);

            if (livro == null)
            {

                _logger.LogWarning(
                    "Livro {id} não encontrada para edição",
                    id
                );

                return ServiceResult<bool>.Error(
                        false,
                        "Não foi encontrado o livro com ID: " + id,
                        ResultType.NotFound
                    );
            }

            var nomeFormatado = PadronizarTexto(livroEditDTO.Nome);

            _logger.LogInformation(
                "Verificando existência do livro com mesmo nome: {nome}",
                nomeFormatado
            );

            var nomeJaExiste = await _repo.ExisteLivroComEsseNomeAsync(nomeFormatado, id);

            if (nomeJaExiste)
            {
                _logger.LogWarning(
                    "Tentativa de criar livro duplicado com nome {nome}",
                    nomeFormatado
                );

                return ServiceResult<bool>.Error(
                    false,
                    "Livros com mesmo nome já adicionado",
                    ResultType.Conflito
                );
            }

            _logger.LogInformation(
                "Verificando a validade das categorias com id: {categoriasId}",
                livroEditDTO.CategoriasId
            );

            var categorias = await _categoriaRepo.ObterCategoriasValidasAsync(livroEditDTO.CategoriasId!);

            if (categorias.Count != livroEditDTO.CategoriasId!.Count)
            {
                _logger.LogWarning(
                    "Uma ou mais categorias não foram encontradas: {id}",
                    livroEditDTO.CategoriasId
                );

                return ServiceResult<bool>.Error(
                        false,
                        "Categoria não encontrada",
                        ResultType.NotFound
                    );
            }

            _logger.LogInformation(
                "Atualizando livro {id}",
                id
            );

            livro.Nome = nomeFormatado;
            livro.Quantidade = livroEditDTO.Quantidade;
            livro.Categorias = categorias;
            livro.DataUltimaAtualizacao = DateTime.UtcNow;

            _repo.Atualizar(livro);
            await _repo.SalvarAsync();

            _logger.LogInformation(
                "Livro {id} editada com sucesso",
                id
            );

            return ServiceResult<bool>.Success(
                true,
                "Livro editado com sucesso",
                ResultType.Atualizado
            );
        }

        public async Task<ServiceResult<LivroResponseDTO>> adicionarAsync(LivroCreateDTO livroCreateDTO)
        {

            _logger.LogInformation(
                "Usuário {UserId} solicitou criação de novo livro. Admin: {IsAdmin}",
                _currentUser.userId,
                _currentUser.IsAdmin
            );

            if (!_currentUser.IsAdmin)
            {
                _logger.LogWarning(
                    "Usuário {UserId} tentou criar livro sem permissão",
                    _currentUser.userId
                );

                return ServiceResult<LivroResponseDTO>.Error(
                        null,
                        "Apenas administradores podem criar livros",
                        ResultType.NaoAutorizado
                    );
            }

            if (livroCreateDTO == null)
            {
                _logger.LogWarning(
                    "Usuário {UserId} enviou DTO nulo para criação de livro",
                    _currentUser.userId
                );

                return ServiceResult<LivroResponseDTO>.Error(
                        null,
                        "É obrigatorio que o DTO Criar não seja nulo",
                        ResultType.Invalido
                    );
            }

            var nomeFormatado = PadronizarTexto(livroCreateDTO.Nome);

            _logger.LogInformation(
                "Verificando se já existe livro com nome {nome}",
                nomeFormatado
            );

            var livroComMesmoNome = await _repo.ExisteLivroComEsseNomeAsync(nomeFormatado);

            if (livroComMesmoNome)
            {
                _logger.LogWarning(
                    "Tentativa de criação de livro duplicado com nome {nome}",
                    nomeFormatado
                );

                return ServiceResult<LivroResponseDTO>.Error(
                        null,
                        "Livros com mesmo nome já adicionado",
                        ResultType.Conflito
                    );
            }

            _logger.LogInformation(
                "Verificando a validade das categorias com id: {categoriasId}",
                livroCreateDTO.CategoriasId
            );

            var categorias = await _categoriaRepo.ObterCategoriasValidasAsync(livroCreateDTO.CategoriasId!);

            if (categorias.Count != livroCreateDTO.CategoriasId!.Count)
            {
                _logger.LogWarning(
                    "Uma ou mais categorias não foram encontradas: {id}",
                    livroCreateDTO.CategoriasId
                );

                return ServiceResult<LivroResponseDTO>.Error(
                    null,
                    "Categoria não encontrada",
                    ResultType.NotFound
                );
            }

            var novoLivroEntity = new Livro(
                nomeFormatado,
                livroCreateDTO.Quantidade,
                _currentUser.userId
            )
            {
                Categorias = categorias,
                DataCriacao = DateTime.UtcNow,
                DataUltimaAtualizacao = DateTime.UtcNow,
                Ativo = true
            };

            await _repo.AdicionarAsync(novoLivroEntity);
            await _repo.SalvarAsync();

            _logger.LogInformation(
                "Livro {id} criada com sucesso",
                novoLivroEntity.Id
            );

            return ServiceResult<LivroResponseDTO>.Success(
                _mapper.Map<LivroResponseDTO>(novoLivroEntity),
                "Livro adicionado com sucesso",
                ResultType.Criado
            );
        }
    }
}
