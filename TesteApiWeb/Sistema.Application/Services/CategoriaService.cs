using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Models;
using BibliotecaWebApiRest.Repositories.Interfaces;
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

        public CategoriaService(ICategoriaRepository repo, ICurrentUser currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
        }

        public async Task<ServiceResult<IEnumerable<CategoriaResponseDTO>>> listarAsync()
        {

            var categorias = await _repo.ObterTodosAsync(_currentUser.IsAdmin);

            return ServiceResult<IEnumerable<CategoriaResponseDTO>>.SuccessList(
                categorias.Select(Mapear),
                "Categorias localizadas com sucesso",
                ResultType.Sucesso
            );
        }

        public async Task<ServiceResult<CategoriaResponseDTO>> buscarPorId(int id)
        {
            var categoria = await _repo.ObterPorIdAsync(id, _currentUser.IsAdmin);

            if (categoria == null)
                return ServiceResult<CategoriaResponseDTO>.Error(
                    null,
                    "Nenhuma categoria encontrada com esse ID: " + id,
                    ResultType.NotFound
                );

            return ServiceResult<CategoriaResponseDTO>.Success(
                Mapear(categoria),
                "Categoria localizada com ID: " + id,
                ResultType.Sucesso
            );
        }

        public async Task<ServiceResult<bool>> softDeletePorId(int id)
        {

            if (!_currentUser.IsAdmin)
                return ServiceResult<bool>.Error(
                    false,
                    "Apenas administradores podem apagar categorias",
                    ResultType.NaoAutorizado
                );

            var categoria = await _repo.ObterPorIdAsync(id, _currentUser.IsAdmin);

            if (categoria == null)
                return ServiceResult<bool>.Error(
                    false,
                    "Nenhuma categoria encontrada com esse ID: " + id,
                    ResultType.NotFound
                );

            if (await _repo.ExisteCategoriaVinculadaAoLivroAsync(id))
                return ServiceResult<bool>.Error(
                    false,
                    "Existem categorias vinculadas á livros",
                    ResultType.Conflito
                );
            
            _repo.SoftDelete(categoria);
            await _repo.SalvarAsync();

            return ServiceResult<bool>.Success(
                true,
                "Categoria excluida com sucesso",
                ResultType.Sucesso
            );
        }

        public async Task<ServiceResult<bool>> editarAsync(int id, CategoriaUpdateDTO TDtoEdit)
        {

            if (!_currentUser.IsAdmin)
                return ServiceResult<bool>.Error(
                    false,
                    "Apenas administradores podem editar categorias",
                    ResultType.NaoAutorizado
                );

            if (TDtoEdit == null)
                return ServiceResult<bool>.Error(
                    false,
                    "É obrigatorio que o DTO Update não seja nulo",
                    ResultType.Invalido
                );

            var categoriaExiste = await _repo.ObterPorIdAsync(id, _currentUser.IsAdmin);
            if (categoriaExiste == null) 
                return ServiceResult<bool>.Error(
                    false,
                    "Nenhuma categoria encontrada com esse ID: " + id,
                    ResultType.NotFound
                );


            var nomeFormatado = PadronizarTexto(TDtoEdit.Nome);
            var categoriaComMesmoNome = await _repo.ExisteCategoriaComEsseNomeAsync(nomeFormatado, id);

            if (categoriaComMesmoNome)
                return ServiceResult<bool>.Error(
                    false,
                    "Já existe uma categoria com mesmo nome: ",
                    ResultType.Conflito
                );

            categoriaExiste.Nome = nomeFormatado;
            _repo.Atualizar(categoriaExiste);
            await _repo.SalvarAsync();
            

            return ServiceResult<bool>.Success(
                true,
                "Categoria editada com sucesso",
                ResultType.Atualizado
            );
        }

        public async Task<ServiceResult<CategoriaResponseDTO>> adicionarAsync(CategoriaCreateDTO categoriaCreateDTO)
        {

            if (!_currentUser.IsAdmin)
                return ServiceResult<CategoriaResponseDTO>.Error(
                    null,
                    "Apenas administradores podem criar categorias",
                    ResultType.NaoAutorizado
                );

            if (categoriaCreateDTO == null)
                return ServiceResult<CategoriaResponseDTO>.Error(
                    null,
                    "É obrigatorio que o DTO Create não seja nulo",
                    ResultType.Invalido
                );

            var nomeFormatado = PadronizarTexto(categoriaCreateDTO.Nome);

            var categoriaComMesmoNome = await _repo.ExisteCategoriaComEsseNomeAsync(nomeFormatado);

            if (categoriaComMesmoNome) 
                return ServiceResult<CategoriaResponseDTO>.Error(
                    null,
                    "Já existe uma categoria com mesmo nome: ",
                    ResultType.Conflito
                );


            var novaCategoria = new Categoria { Nome = nomeFormatado, Ativo = true };

            await _repo.AdicionarAsync(novaCategoria);
            await _repo.SalvarAsync();

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