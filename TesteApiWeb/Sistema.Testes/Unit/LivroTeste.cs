using AutoMapper;
using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Models;
using BibliotecaWebApiRest.Repositories.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Sistema.Application.Commoms.Pagination;
using Sistema.Application.Interfaces;
using TesteApiWeb.Services;
using static DTOS.Categoria.CategoriaDTO;
using static DTOS.Livro.LivroDTO;

namespace Sistema.Testes.Unit
{
    public class LivroTeste
    {

        private readonly LivroService _livroService;
        private readonly Mock<ICurrentUser> _mockUser;
        private readonly Mock<ILivroRepository> _mockLivroRepo;
        private readonly Mock<ICategoriaRepository> _mockCategoriaRepo;
        private readonly Livro livro;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<LivroService>> _mockLogger;

        public LivroTeste()
        {
            _mockUser = new Mock<ICurrentUser>();
            _mockLivroRepo = new Mock<ILivroRepository>();
            _mockCategoriaRepo = new Mock<ICategoriaRepository>();
            _mockLogger = new Mock<ILogger<LivroService>>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Livro, LivroResponseDTO>();
                cfg.CreateMap<Categoria, CategoriaResponseDTO>();
            });

            _mapper = config.CreateMapper();

            _livroService = new LivroService(
                _mockLivroRepo.Object,
                _mockCategoriaRepo.Object,
                _mockUser.Object,
                _mapper,
                _mockLogger.Object
             );

            livro = new Livro { Nome = "Teste", Categorias = new List<Categoria>(), IdUsuarioAdminCriou = "userTeste", Quantidade = 10 };

        }

        [Fact]
        public async Task BuscarLivroPorId_DeveDarSucesso_IdValido()
        {

            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockLivroRepo
                .Setup(x => x.ObterLivroIncluindoCategoriaPorId(It.IsAny<int>(), true))
                .ReturnsAsync(livro);

            var result = await _livroService.buscarPorId(1);

            result.Sucesso.Should().BeTrue();
            result.Tipo.Should().Be(ResultType.Sucesso);

            _mockLivroRepo.Verify(x => x.ObterLivroIncluindoCategoriaPorId(1, true), Times.Once);

        }

        [Fact]
        public async Task BuscarLivroPorId_DeveDarErro_LivroNaoEncontrado()
        {

            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockLivroRepo.Setup(x => x.ObterLivroIncluindoCategoriaPorId(It.IsAny<int>(), true)).ReturnsAsync((Livro?)null);

            var result = await _livroService.buscarPorId(1);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NotFound);

            _mockLivroRepo.Verify(x => x.ObterLivroIncluindoCategoriaPorId(It.IsAny<int>(), true), Times.Once);

        }

        [Fact]
        public async Task BuscarLivroPorId_DeveDarErro_IdValidoMasLivroDesativadoClienteLogado()
        {
            _mockUser.Setup(x => x.IsAdmin).Returns(false);

            _mockLivroRepo.Setup(x => x.ObterLivroIncluindoCategoriaPorId(It.IsAny<int>(), false)).ReturnsAsync((Livro?)null);

            var result = await _livroService.buscarPorId(1);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NotFound);

            _mockLivroRepo.Verify(x => x.ObterLivroIncluindoCategoriaPorId(It.IsAny<int>(), false), Times.Once);

        }

        [Fact]
        public async Task SoftDeleteLivro_DeveDarSucesso_AdminLogado()
        {
            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockLivroRepo.Setup(x => x.ObterPorIdAsync(It.IsAny<int>(), true)).ReturnsAsync(livro);

            _mockLivroRepo.Setup(x => x.SalvarAsync()).Returns(Task.CompletedTask);

            var result = await _livroService.softDeletePorId(1);

            result.Sucesso.Should().BeTrue();
            result.Tipo.Should().Be(ResultType.Sucesso);

            _mockLivroRepo.Verify(x => x.ObterPorIdAsync(It.IsAny<int>(), true), Times.Once);
            _mockLivroRepo.Verify(x => x.SalvarAsync(), Times.Once);
            _mockLivroRepo.Verify(x => x.SoftDelete(It.IsAny<Livro>()), Times.Once);
        }

        [Fact]
        public async Task SoftDeleteLivro_DeveDarErro_ClienteLogado()
        {
            var livroProcurado = new Livro { Nome = "Teste", Categorias = new List<Categoria>(), IdUsuarioAdminCriou = "userTeste", Quantidade = 10 };

            _mockUser.Setup(x => x.IsAdmin).Returns(false);

            var result = await _livroService.softDeletePorId(1);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NaoAutorizado);

            _mockLivroRepo.Verify(x => x.SoftDelete(It.IsAny<Livro>()), Times.Never);
            _mockLivroRepo.Verify(x => x.SalvarAsync(), Times.Never);

        }

        [Fact]
        public async Task SoftDeleteLivro_DeveDarErro_AdminLogadoMasLivroNaoEncontrado()
        {
            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockLivroRepo.Setup(x => x.ObterPorIdAsync(It.IsAny<int>(), true)).ReturnsAsync((Livro?)null);

            var result = await _livroService.softDeletePorId(1);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NotFound);

            _mockLivroRepo.Verify(x => x.ObterPorIdAsync(It.IsAny<int>(), true), Times.Once);
            _mockLivroRepo.Verify(x => x.SoftDelete(It.IsAny<Livro>()), Times.Never);

        }

        [Fact]
        public async Task EditarLivroPorId_DeveDarSucesso_AdminLogado()
        {

            var categoriaValida = new List<Categoria>();

            var livroDto = new LivroEditDTO
            {
                Nome = "Livro Teste",
                Quantidade = 10,
                CategoriasId = new List<int>()
            };

            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockLivroRepo.Setup(x => x.ObterPorIdAsync(It.IsAny<int>(), true)).ReturnsAsync(livro);

            _mockLivroRepo.Setup(x => x.ExisteLivroComEsseNomeAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(false);

            _mockCategoriaRepo
                .Setup(x => x.ObterCategoriasValidasAsync(It.IsAny<ICollection<int>>()))
                .ReturnsAsync(categoriaValida);

            var result = await _livroService.editarAsync(1, livroDto);

            result.Sucesso.Should().BeTrue();
            result.Tipo.Should().Be(ResultType.Atualizado);

            _mockLivroRepo.Verify(x => x.Atualizar(livro), Times.Once);
            _mockLivroRepo.Verify(x => x.SalvarAsync(), Times.Once);

        }

        [Fact]
        public async Task EditarLivroPorId_DeveDarErro_AdminLogadoMasLivroNaoEncontrado()
        {
            var livroDto = new LivroEditDTO
            {
                Nome = "Livro Teste",
                Quantidade = 10,
                CategoriasId = new List<int>()
            };

            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockLivroRepo
                .Setup(x => x.ObterPorIdAsync(It.IsAny<int>(), true))
                .ReturnsAsync((Livro?)null);

            var result = await _livroService.editarAsync(1, livroDto);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NotFound);

            _mockLivroRepo.Verify(x => x.SalvarAsync(), Times.Never);
            _mockLivroRepo.Verify(x => x.Atualizar(It.IsAny<Livro>()), Times.Never);

        }

        [Fact]
        public async Task EditarLivroPorId_DeveDarErro_AdminLogadoMasDadosDeLivroNull()
        {

            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockLivroRepo.Setup(x => x.ObterPorIdAsync(It.IsAny<int>(), true)).ReturnsAsync((Livro?)null);

            var result = await _livroService.editarAsync(1, null);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.Invalido);

            _mockLivroRepo.Verify(x => x.ObterPorIdAsync(1, true), Times.Never);

        }

        [Fact]
        public async Task EditarLivroPorId_DeveDarErro_ClienteLogado()
        {
            var livroDto = new LivroEditDTO();

            _mockUser.Setup(x => x.IsAdmin).Returns(false);

            var result = await _livroService.editarAsync(1, livroDto);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NaoAutorizado);

            _mockLivroRepo.Verify(x => x.ObterPorIdAsync(1, true), Times.Never);
        }

        [Fact]
        public async Task EditarLivroPorId_DeveDarErro_AdminLogadoMasNomeComMesmoNomeExiste()
        {

            var livroDto = new LivroEditDTO();

            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockLivroRepo.Setup(x => x.ObterPorIdAsync(It.IsAny<int>(), true)).ReturnsAsync(livro);

            _mockLivroRepo.Setup(x => x.ExisteLivroComEsseNomeAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(true);

            var result = await _livroService.editarAsync(1, livroDto);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.Conflito);

            _mockLivroRepo.Verify(x => x.SalvarAsync(), Times.Never);
            _mockLivroRepo.Verify(x => x.Atualizar(It.IsAny<Livro>()), Times.Never);

        }

        [Fact]
        public async Task EditarLivroPorId_DeveDarErro_AdminLogadoMasCategoriasInvalidas()
        {
            var livroDto = new LivroEditDTO
            {
                Nome = "Livro Teste",
                Quantidade = 10,
                CategoriasId = new List<int> { 1, 2 }
            };

            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockLivroRepo.Setup(x => x.ObterPorIdAsync(It.IsAny<int>(), true)).ReturnsAsync(livro);

            _mockLivroRepo.Setup(x => x.ExisteLivroComEsseNomeAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(false);

            _mockCategoriaRepo
                .Setup(x => x.ObterCategoriasValidasAsync(It.IsAny<ICollection<int>>()))
                .ReturnsAsync(new List<Categoria>
                {
                    new Categoria { Id = 1, Nome = "Categoria 1", Ativo = true }
                });

            var result = await _livroService.editarAsync(1, livroDto);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NotFound);

            _mockLivroRepo.Verify(x => x.SalvarAsync(), Times.Never);
            _mockLivroRepo.Verify(x => x.Atualizar(It.IsAny<Livro>()), Times.Never);

        }

        [Fact]
        public async Task AdicionarLivroAsync_DeveDarSucesso_DadosValidosEAdmin()
        {
            var livroCreateDto = new LivroCreateDTO
            {
                Nome = "Livro Teste",
                Quantidade = 10,
                CategoriasId = new List<int> { 1, 2 },
                
            };

            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockUser.Setup(x => x.userId).Returns("user 123");

            _mockLivroRepo
                .Setup(x => x.ExisteLivroComEsseNomeAsync(It.IsAny<string>(), null))
                .ReturnsAsync(false);

            _mockCategoriaRepo
                .Setup(x => x.ObterCategoriasValidasAsync(It.IsAny<ICollection<int>>()))
                .ReturnsAsync(new List<Categoria>
                {
                    new Categoria { Id = 1, Nome = "Categoria 1", Ativo = true, IdUsuarioCriacao = "user 123" },
                    new Categoria { Id = 2, Nome = "Categoria 2", Ativo = true, IdUsuarioCriacao = "user 123", }
                });

            var result = await _livroService.adicionarAsync(livroCreateDto);

            result.Sucesso.Should().BeTrue();
            result.Tipo.Should().Be(ResultType.Criado);

            _mockLivroRepo.Verify(x => x.AdicionarAsync(It.IsAny<Livro>()), Times.Once);
            _mockLivroRepo.Verify(x => x.SalvarAsync(), Times.Once);
        }

        [Fact]
        public async Task AdicionarLivroAsync_DeveDarErro_ClienteLogado()
        {
            var livroCreateDto = new LivroCreateDTO
            {
                Nome = "Livro Teste",
                Quantidade = 10,
                CategoriasId = new List<int> { 1, 2 }
            };

            _mockUser.Setup(x => x.IsAdmin).Returns(false);

            var result = await _livroService.adicionarAsync(livroCreateDto);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NaoAutorizado);

        }

        [Fact]
        public async Task AdicionarLivroAsync_DeveDarErro_AdminLogadoMasLivroComMesmoNome()
        {
            var livroCreateDto = new LivroCreateDTO
            {
                Nome = "Livro Teste",
                Quantidade = 10,
                CategoriasId = new List<int> { 1, 2 }
            };

            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockLivroRepo.Setup(x => x.ExisteLivroComEsseNomeAsync(It.IsAny<string>(), null)).ReturnsAsync(true);

            var result = await _livroService.adicionarAsync(livroCreateDto);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.Conflito);

            _mockLivroRepo.Verify(x => x.SalvarAsync(), Times.Never);

        }


        [Fact]
        public async Task AdicionarLivro_DeveDarErro_AdminLogadoMasCategoriasInvalidas()
        {
            var livroDto = new LivroCreateDTO
            {
                Nome = "Livro Teste",
                Quantidade = 10,
                CategoriasId = new List<int> { 1, 2 }
            };

            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockLivroRepo.Setup(x => x.ExisteLivroComEsseNomeAsync(It.IsAny<string>(), null)).ReturnsAsync(false);

            _mockCategoriaRepo
                .Setup(x => x.ObterCategoriasValidasAsync(It.IsAny<ICollection<int>>()))
                .ReturnsAsync(new List<Categoria>
                {
                    new Categoria { Id = 1, Nome = "Categoria 1", Ativo = true },

                });

            var result = await _livroService.adicionarAsync(livroDto);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NotFound);

            _mockLivroRepo.Verify(x => x.SalvarAsync(), Times.Never);
            _mockLivroRepo.Verify(x => x.Atualizar(It.IsAny<Livro>()), Times.Never);
        }

        }
    }
