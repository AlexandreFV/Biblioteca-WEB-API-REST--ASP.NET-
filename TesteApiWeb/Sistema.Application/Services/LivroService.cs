using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Models;
using BibliotecaWebApiRest.Repositories.Interfaces;
using DTOS.Livro;
using Microsoft.EntityFrameworkCore;
using Sistema.Application.Commoms.Bases;
using Sistema.Application.Interfaces;
using Sistema.Application.Interfaces.Services;
using Sistema.Application.Services;
using System.Security.Claims;
using System.Security.Principal;
using static DTOS.Categoria.CategoriaDTO;
using static DTOS.Livro.LivroDTO;

namespace TesteApiWeb.Services
{
    public class LivroService : PadronizarTextoService, ILivroService
    {

        private readonly ILivroRepository _repo;
        private readonly ICategoriaRepository _categoriaRepo;
        private readonly ICurrentUser _currentUser;

        public LivroService(ILivroRepository livroRepository, ICategoriaRepository categoriaRepository, ICurrentUser currentUser) { _repo = livroRepository; _categoriaRepo = categoriaRepository; _currentUser = currentUser;  }

        private LivroResponseDTO Mapear(Livro l) 
        {
            var LivroMapeado = new LivroResponseDTO
            {
                Id = l.Id,
                Nome = l.Nome,
                Quantidade = l.Quantidade,
                Categorias = l.Categorias!.Select(c => new CategoriaResponseDTO
                {
                    Id = c.Id,
                    Nome = c.Nome
                }).ToList()
            };

            return LivroMapeado;
        }

        public async Task<ServiceResult<IEnumerable<LivroResponseDTO>>> listarAsync()
        {
            var livros = await _repo.ObterTodosLivrosIncluindoCategoria(_currentUser.IsAdmin);

            return ServiceResult<IEnumerable<LivroResponseDTO>>.SuccessList(
                livros.Select(Mapear),
                "Livros localizadas com sucesso",
                ResultType.Sucesso
            );
        }

        public async Task<ServiceResult<LivroResponseDTO>> buscarPorId(int id)
        {
            var livro = await _repo.ObterLivroIncluindoCategoriaPorId(id, _currentUser.IsAdmin);

            if (livro == null || (!_currentUser.IsAdmin && (!livro.Ativo || livro.Quantidade <= 0)))
                return ServiceResult<LivroResponseDTO>.Error(
                    null,
                    "Não foi encontrado o livro com ID: " + id,
                    ResultType.NotFound
                );

            return ServiceResult<LivroResponseDTO>.Success(
                Mapear(livro),
                "Livro encontrado com ID: " + id,
                ResultType.Sucesso
            );
        }

        public async Task<ServiceResult<bool>> softDeletePorId(int id)
        {

            if (!_currentUser.IsAdmin)
                return ServiceResult<bool>.Error(
                    false,
                    "Apenas administradores podem apagar livros",
                    ResultType.NaoAutorizado
                );

            var livro = await _repo.ObterPorIdAsync(id, _currentUser.IsAdmin);

            if (livro == null)
                return ServiceResult<bool>.Error(
                    false,
                    "Não foi encontrado o livro com ID: " + id,
                    ResultType.NotFound
                );

            _repo.SoftDelete(livro);
            await _repo.SalvarAsync();

            return ServiceResult<bool>.Success(
                true,
                "Livro apagado com sucesso",
                ResultType.Sucesso
            );
        }

        public async Task<ServiceResult<bool>> editarAsync(int id, LivroEditDTO livroEditDTO)
        {

            if (!_currentUser.IsAdmin)
                return ServiceResult<bool>.Error(
                    false,
                    "Apenas administradores podem editar livros",
                    ResultType.NaoAutorizado
                );

            if (livroEditDTO == null)
                return ServiceResult<bool>.Error(
                    false,
                    "É obrigatorio que o DTO Editar não seja nulo",
                    ResultType.Invalido
                );

            var livro = await _repo.ObterPorIdAsync(id, _currentUser.IsAdmin);

            if (livro == null)
                return ServiceResult<bool>.Error(
                    false,
                    "Não foi encontrado o livro com ID: " + id,
                    ResultType.NotFound
                );

            var nomeFormatado = PadronizarTexto(livroEditDTO.Nome);

            var nomeJaExiste = await _repo.ExisteLivroComEsseNomeAsync(nomeFormatado, id);

            if (nomeJaExiste)
                return ServiceResult<bool>.Error(
                    false,
                    "Livros com mesmo nome já adicionado",
                    ResultType.Conflito
                );

            var categorias = await _categoriaRepo.ObterCategoriasValidasAsync(livroEditDTO.CategoriasId!);

            if (categorias.Count != livroEditDTO.CategoriasId!.Count)
                return ServiceResult<bool>.Error(
                    false,
                    "Categoria não encontrada",
                    ResultType.NotFound
                );

            livro.Nome = nomeFormatado;
            livro.Quantidade = livroEditDTO.Quantidade;
            livro.Categorias = categorias;

            _repo.Atualizar(livro);
            await _repo.SalvarAsync();

            return ServiceResult<bool>.Success(
                true,
                "Livro editado com sucesso",
                ResultType.Atualizado
            );
        }

        public async Task<ServiceResult<LivroResponseDTO>> adicionarAsync(LivroCreateDTO livroCreateDTO)
        {

            if (!_currentUser.IsAdmin)
                return ServiceResult<LivroResponseDTO>.Error(
                    null,
                    "Apenas administradores podem criar livros",
                    ResultType.NaoAutorizado
                );

            if (livroCreateDTO == null)
                return ServiceResult<LivroResponseDTO>.Error(
                    null,
                    "É obrigatorio que o DTO Criar não seja nulo",
                    ResultType.Invalido
                );

            var nomeFormatado = PadronizarTexto(livroCreateDTO.Nome);

            var livroComMesmoNome = await _repo.ExisteLivroComEsseNomeAsync(nomeFormatado);

            if (livroComMesmoNome)
                return ServiceResult<LivroResponseDTO>.Error(
                    null,
                    "Livros com mesmo nome já adicionado",
                    ResultType.Conflito
                );

            var categorias = await _categoriaRepo.ObterCategoriasValidasAsync(livroCreateDTO.CategoriasId!);

            if (categorias.Count != livroCreateDTO.CategoriasId!.Count)
                return ServiceResult<LivroResponseDTO>.Error(
                    null,
                    "Categoria não encontrada",
                    ResultType.NotFound
                );

            var novoLivroEntity = new Livro
            {
                Nome = nomeFormatado,
                Quantidade = livroCreateDTO.Quantidade,
                Categorias = categorias,
                Ativo = true
            };

            await _repo.AdicionarAsync(novoLivroEntity);
            await _repo.SalvarAsync();

            return ServiceResult<LivroResponseDTO>.Success(
                Mapear(novoLivroEntity),
                "Livro adicionado com sucesso",
                ResultType.Criado
            );
        }
    }
}
