using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Models;
using BibliotecaWebApiRest.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Sistema.Application.Interfaces;
using Sistema.Application.Interfaces.Services;
using Sistema.Application.Services;
using static DTOS.Categoria.CategoriaDTO;

namespace Biblioteca_WEB_API_REST_ASP.Services
{
    public class CategoriaService : PadronizarTextoService, ICategoriaService
    {
        private readonly ICategoriaRepository _repo;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<CategoriaService> _logger;

        public CategoriaService(ICategoriaRepository repo, ICurrentUser currentUser, ILogger<CategoriaService> logger)
        {
            _repo = repo;
            _currentUser = currentUser;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<CategoriaResponseDTO>>> listarAsync()
        {

            _logger.LogInformation(
                "Usuário {UserId} solicitou listagem de categorias. Admin: {IsAdmin}",
                _currentUser.userId,
                _currentUser.IsAdmin
            );

            var categorias = await _repo.ObterTodosAsync(_currentUser.IsAdmin);

            _logger.LogInformation(
                "Categorias retornadas com sucesso para usuário {UserId}",
                _currentUser.userId
            );

            return ServiceResult<IEnumerable<CategoriaResponseDTO>>.SuccessList(
                categorias.Select(Mapear),
                "Categorias localizadas com sucesso",
                ResultType.Sucesso
            );
        }

        public async Task<ServiceResult<CategoriaResponseDTO>> buscarPorId(int id)
        {
            _logger.LogInformation(
                "Usuário {UserId} solicitou uma categoria com id: {id}. Admin: {IsAdmin}",
                _currentUser.userId,
                id,
                _currentUser.IsAdmin
            );

            var categoria = await _repo.ObterPorIdAsync(id, _currentUser.IsAdmin);

            if (categoria == null)
            {
                _logger.LogWarning(
                    "Categoria com id: {id} não foi encontrada para o user {userId}",
                    id,
                    _currentUser.userId
                );
                return ServiceResult<CategoriaResponseDTO>.Error(
                        null,
                        "Nenhuma categoria encontrada com esse ID: " + id,
                        ResultType.NotFound
                    );
            }

            _logger.LogInformation(
                "Categoria com id: {id} encontrada com sucesso para o user {userId}",
                id,
                _currentUser.userId
            );

            return ServiceResult<CategoriaResponseDTO>.Success(
                Mapear(categoria),
                "Categoria localizada com ID: " + id,
                ResultType.Sucesso
            );
        }

        public async Task<ServiceResult<bool>> softDeletePorId(int id)
        {

            _logger.LogInformation(
                "Usuário {UserId} solicita a exclusão da categoria com id: {id}. Admin: {IsAdmin}",
                _currentUser.userId,
                id,
                _currentUser.IsAdmin
            );

            if (!_currentUser.IsAdmin)
            {
                _logger.LogWarning(
                    "Usuário {UserId} não possui permissão para excluir a categoria com id: {id}",
                    _currentUser.userId,
                    id
                );

                return ServiceResult<bool>.Error(
                        false,
                        "Apenas administradores podem apagar categorias",
                        ResultType.NaoAutorizado
                    );
            }

            _logger.LogInformation(
                "Iniciando procura de categoria com base no id {CategoriaId}",
                id
            );

            var categoria = await _repo.ObterPorIdAsync(id, _currentUser.IsAdmin);

            if (categoria == null)
            {
                _logger.LogWarning(
                    "Categoria {CategoriaId} não encontrado",
                    id
                );

                return ServiceResult<bool>.Error(
                    false,
                    "Nenhuma categoria encontrada com esse ID: " + id,
                    ResultType.NotFound
                );

            }

            _logger.LogInformation(
                "Iniciando verificação de vinculo entre categoria {CategoriaId} e livros",
                id
            );

            if (await _repo.ExisteCategoriaVinculadaAoLivroAsync(id))
            {

                _logger.LogWarning(
                    "Existem livros vinculados á categoria {CategoriaId}",
                    id
                );

                return ServiceResult<bool>.Error(
                    false,
                    "Existem categorias vinculadas á livros",
                    ResultType.Conflito
                );


            }

            _logger.LogInformation(
                "Iniciando soft delete da categoria {CategoriaId}",
                id
            );

            _repo.SoftDelete(categoria);
            await _repo.SalvarAsync();

            _logger.LogInformation(
                "Soft delete da categoria {CategoriaId} realizado com sucesso",
                id
            );

            return ServiceResult<bool>.Success(
                true,
                "Categoria excluida com sucesso",
                ResultType.Sucesso
            );
        }

        public async Task<ServiceResult<bool>> editarAsync(int id, CategoriaUpdateDTO TDtoEdit)
        {
            _logger.LogInformation(
                "Usuário {UserId} solicitou edição da categoria {CategoriaId}. Admin: {IsAdmin}",
                _currentUser.userId,
                id,
                _currentUser.IsAdmin
            );

            if (!_currentUser.IsAdmin)
            {
                _logger.LogWarning(
                    "Usuário {UserId} tentou editar a categoria {CategoriaId} sem permissão",
                    _currentUser.userId,
                    id
                );

                return ServiceResult<bool>.Error(
                    false,
                    "Apenas administradores podem editar categorias",
                    ResultType.NaoAutorizado
                );
            }

            if (TDtoEdit == null)
            {
                _logger.LogWarning(
                    "Usuário {UserId} enviou DTO nulo para edição da categoria {CategoriaId}",
                    _currentUser.userId,
                    id
                );

                return ServiceResult<bool>.Error(
                    false,
                    "É obrigatorio que o DTO Update não seja nulo",
                    ResultType.Invalido
                );
            }

            _logger.LogInformation(
                "Buscando categoria {CategoriaId} para edição",
                id
            );

            var categoriaExiste = await _repo.ObterPorIdAsync(id, _currentUser.IsAdmin);

            if (categoriaExiste == null)
            {
                _logger.LogWarning(
                    "Categoria {CategoriaId} não encontrada para edição",
                    id
                );

                return ServiceResult<bool>.Error(
                    false,
                    "Nenhuma categoria encontrada com esse ID: " + id,
                    ResultType.NotFound
                );
            }

            var nomeFormatado = PadronizarTexto(TDtoEdit.Nome);

            _logger.LogInformation(
                "Verificando existência de categoria com mesmo nome: {NomeCategoria}",
                nomeFormatado
            );

            var categoriaComMesmoNome = await _repo.ExisteCategoriaComEsseNomeAsync(nomeFormatado, id);

            if (categoriaComMesmoNome)
            {
                _logger.LogWarning(
                    "Tentativa de criar categoria duplicada com nome {NomeCategoria}",
                    nomeFormatado
                );

                return ServiceResult<bool>.Error(
                    false,
                    "Já existe uma categoria com mesmo nome",
                    ResultType.Conflito
                );
            }

            _logger.LogInformation(
                "Atualizando categoria {CategoriaId}",
                id
            );

            categoriaExiste.Nome = nomeFormatado;
            _repo.Atualizar(categoriaExiste);
            await _repo.SalvarAsync();

            _logger.LogInformation(
                "Categoria {CategoriaId} editada com sucesso",
                id
            );

            return ServiceResult<bool>.Success(
                true,
                "Categoria editada com sucesso",
                ResultType.Atualizado
            );
        }

        public async Task<ServiceResult<CategoriaResponseDTO>> adicionarAsync(CategoriaCreateDTO categoriaCreateDTO)
        {
            _logger.LogInformation(
                "Usuário {UserId} solicitou criação de nova categoria. Admin: {IsAdmin}",
                _currentUser.userId,
                _currentUser.IsAdmin
            );

            if (!_currentUser.IsAdmin)
            {
                _logger.LogWarning(
                    "Usuário {UserId} tentou criar categoria sem permissão",
                    _currentUser.userId
                );

                return ServiceResult<CategoriaResponseDTO>.Error(
                    null,
                    "Apenas administradores podem criar categorias",
                    ResultType.NaoAutorizado
                );
            }

            if (categoriaCreateDTO == null)
            {
                _logger.LogWarning(
                    "Usuário {UserId} enviou DTO nulo para criação de categoria",
                    _currentUser.userId
                );

                return ServiceResult<CategoriaResponseDTO>.Error(
                    null,
                    "É obrigatorio que o DTO Create não seja nulo",
                    ResultType.Invalido
                );
            }

            var nomeFormatado = PadronizarTexto(categoriaCreateDTO.Nome);

            _logger.LogInformation(
                "Verificando se já existe categoria com nome {NomeCategoria}",
                nomeFormatado
            );

            var categoriaComMesmoNome = await _repo.ExisteCategoriaComEsseNomeAsync(nomeFormatado);

            if (categoriaComMesmoNome)
            {
                _logger.LogWarning(
                    "Tentativa de criação de categoria duplicada com nome {NomeCategoria}",
                    nomeFormatado
                );

                return ServiceResult<CategoriaResponseDTO>.Error(
                    null,
                    "Já existe uma categoria com mesmo nome",
                    ResultType.Conflito
                );
            }

            _logger.LogInformation(
                "Criando nova categoria com nome {NomeCategoria}",
                nomeFormatado
            );

            var novaCategoria = new Categoria
            {
                Nome = nomeFormatado,
                Ativo = true
            };

            await _repo.AdicionarAsync(novaCategoria);
            await _repo.SalvarAsync();

            _logger.LogInformation(
                "Categoria {CategoriaId} criada com sucesso",
                novaCategoria.Id
            );

            return ServiceResult<CategoriaResponseDTO>.Success(
                Mapear(novaCategoria),
                "Categoria adicionada com sucesso",
                ResultType.Criado
            );
        }

        private CategoriaResponseDTO Mapear(Categoria c)
        {
            var categoriaMapeada = new CategoriaResponseDTO
            {
                Id = c.Id,
                Nome = c.Nome,
            };

            return categoriaMapeada;
        }

    }
}