using AutoMapper;
using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Models;
using Biblioteca_WEB_API_REST_ASP.Services;
using BibliotecaWebApiRest.Repositories.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Sistema.Application.Interfaces;
using static DTOS.Categoria.CategoriaDTO;

namespace Sistema.Testes.Unit
{
    public class CategoriaTeste
    {

        private readonly CategoriaService _categoriaService;
        private readonly Mock<ICurrentUser> _mockUser;
        private readonly Mock<ICategoriaRepository> _mockRepoCategoria;
        private readonly Mock<ILogger<CategoriaService>> _mockLogger;
        private readonly Mock<IMapper> _mockMapper;

        public CategoriaTeste() 
        {
            _mockUser = new Mock<ICurrentUser>();
            _mockRepoCategoria = new Mock<ICategoriaRepository>();
            _mockLogger = new Mock<ILogger<CategoriaService>>();
            _mockMapper = new Mock<IMapper>();

            _categoriaService = new CategoriaService(
                _mockRepoCategoria.Object,
                _mockUser.Object,
                _mockLogger.Object,
                _mockMapper.Object
                );
        }

        [Fact]
        public async Task AdicionarCategoriaAsync_DeveCriarCategoria_QuandoDadosValidos()
        {

            //Arrange 
            var dto = new CategoriaCreateDTO { Nome = "Categoria Teste" };

            _mockUser.Setup(x => x.IsAdmin).Returns(true);
            _mockUser.Setup(x => x.userId).Returns("userId");

            _mockMapper.Setup(x => x.Map<Categoria>(dto))
               .Returns(new Categoria("Categoria Teste", "userId"));

            _mockMapper.Setup(x => x.Map<CategoriaResponseDTO>(It.IsAny<Categoria>()))
               .Returns(new CategoriaResponseDTO { Nome = "Categoria Teste" });

            _mockRepoCategoria.Setup(x => x.ExisteCategoriaComEsseNomeAsync(It.IsAny<string>(), null)).ReturnsAsync(false);

            _mockRepoCategoria.Setup(x => x.AdicionarAsync(It.IsAny<Categoria>())).Returns(Task.CompletedTask);

            _mockRepoCategoria.Setup(x => x.SalvarAsync()).Returns(Task.CompletedTask);

            //Action
            var result = await _categoriaService.adicionarAsync(dto);

            //Assert

            result.Sucesso.Should().BeTrue();
            result.Tipo.Should().Be(ResultType.Criado);
            result.Dados.Should().NotBeNull();

            _mockRepoCategoria.Verify(x => x.AdicionarAsync(It.Is<Categoria>(c =>
                c.Nome == "Categoria Teste" &&
                c.Ativo == true &&
                c.IdUsuarioCriacao == "userId"
            )), Times.Once());

            _mockRepoCategoria.Verify(x => x.SalvarAsync(), Times.Once());
        }

        [Theory]
        [InlineData(false, "Teste", ResultType.NaoAutorizado)]
        [InlineData(true, null, ResultType.Invalido)]
        public async Task AdicionarCategoriaAsync_DeveDarErro_QuandoNaoAdminOuCategoriaNula(bool isAdmin, string nomeCategoria, ResultType esperado)
        {
            var dto = nomeCategoria == null ? null : new CategoriaCreateDTO { Nome = nomeCategoria };

            _mockUser.Setup(x => x.IsAdmin).Returns(isAdmin);

            var result = await _categoriaService.adicionarAsync(dto);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(esperado);

        }

        [Fact]
        public async Task AdicionarCategoriaAsync_DeveDarErro_QuandoJaExisteCategoriaComMesmoNome()
        {
            var dto = new CategoriaCreateDTO { Nome = "Teste" };

            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockRepoCategoria
                .Setup(x => x.ExisteCategoriaComEsseNomeAsync(It.IsAny<string>(), null))
                .ReturnsAsync(true);

            var result = await _categoriaService.adicionarAsync(dto);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.Conflito);
            result.Dados.Should().BeNull();

            _mockRepoCategoria.Verify(x => x.AdicionarAsync(It.IsAny<Categoria>()), Times.Never);
            _mockRepoCategoria.Verify(x => x.SalvarAsync(), Times.Never);

        }

        [Fact]
        public async Task SoftDeleteCategoriaAsync_DeveApagarSucesso_ComPermissao()
        {
            var categoriaProcurada = new Categoria("Teste", "adminId");

            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockRepoCategoria.Setup(x => x.ObterPorIdAsync(It.IsAny<int>(), true)).ReturnsAsync(categoriaProcurada);

            _mockRepoCategoria.Setup(x => x.ExisteCategoriaVinculadaAoLivroAsync(It.IsAny<int>())).ReturnsAsync(false);

            _mockRepoCategoria.Setup(x => x.SalvarAsync()).Returns(Task.CompletedTask);

            var result = await _categoriaService.softDeletePorId(1);

            result.Sucesso.Should().BeTrue();
            result.Tipo.Should().Be(ResultType.Sucesso);

            _mockRepoCategoria.Verify(x => x.SoftDelete(It.IsAny<Categoria>()), Times.Once);
            _mockRepoCategoria.Verify(x => x.SalvarAsync(), Times.Once());

        }

        [Fact]
        public async Task SoftDeleteCategoriaAsync_DeveRetornarErro_QuandoNaoAdmin()
        {
            _mockUser.Setup(x => x.IsAdmin).Returns(false);

            var resul = await _categoriaService.softDeletePorId(1);

            resul.Sucesso.Should().BeFalse();
            resul.Tipo.Should().Be(ResultType.NaoAutorizado);

            _mockRepoCategoria
                .Verify(x => x.ObterPorIdAsync(It.IsAny<int>(), It.IsAny<bool>()),
                Times.Never);

        }

        [Fact]
        public async Task SoftDeleteCategoriaAsync_DeveRetornarErro_QuandoAdminMasNaoExisteCategoria()
        {
            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockRepoCategoria.Setup(x => x.ObterPorIdAsync(It.IsAny<int>(), true)).ReturnsAsync((Categoria?)null);

            var resul = await _categoriaService.softDeletePorId(1);

            resul.Sucesso.Should().BeFalse();
            resul.Tipo.Should().Be(ResultType.NotFound);
        }


        [Fact]
        public async Task SoftDeleteCategoriaAsync_DeveRetornarErro_CategoriaVinculadaLivro()
        {

            var categoria = new Categoria("Teste", "Teste");

            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockRepoCategoria.Setup(x => x.ObterPorIdAsync(It.IsAny<int>(), true)).ReturnsAsync(categoria);
            
            _mockRepoCategoria.Setup(x => x.ExisteCategoriaVinculadaAoLivroAsync(1)).ReturnsAsync(true);

            var result = await _categoriaService.softDeletePorId(1);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.Conflito);

            _mockRepoCategoria.Verify(x => x.SoftDelete(It.IsAny<Categoria>()), Times.Never);
        }

        [Fact]
        public async Task EditarCategoriaAsync_DeveEditar_DadosValidos()
        {
            var dto = new CategoriaUpdateDTO { Nome = "Atualizado" };

            var categoriaEntity = new Categoria("Teste Atualizado", "UserId");

            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockRepoCategoria.Setup(x => x.ObterPorIdAsync(It.IsAny<int>(), true)).ReturnsAsync(categoriaEntity);

            _mockRepoCategoria.Setup(x => x.ExisteCategoriaComEsseNomeAsync(It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(false);

            _mockMapper.Setup(x => x.Map(dto, categoriaEntity))
                .Callback<CategoriaUpdateDTO, Categoria>((d, e) => e.Nome = d.Nome);

            _mockRepoCategoria.Setup(x => x.SalvarAsync()).Returns(Task.CompletedTask);

            var result = await _categoriaService.editarAsync(1, dto);

            result.Sucesso.Should().BeTrue();
            result.Tipo.Should().Be(ResultType.Atualizado);

            _mockRepoCategoria.Verify(x => x.Atualizar(It.Is<Categoria>(c =>
                c.Nome == "Atualizado"
            )), Times.Once);
        }

        [Fact]
        public async Task EditarCategoriaAsync_DeveDarErro_NaoEAdmin()
        {

            _mockUser.Setup(x => x.IsAdmin).Returns(false);

            var result = await _categoriaService.editarAsync(1, new CategoriaUpdateDTO());

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NaoAutorizado);

            _mockRepoCategoria.Verify(x => x.SalvarAsync(), Times.Never);
            _mockRepoCategoria.Verify(x => x.Atualizar(It.IsAny<Categoria>()), Times.Never);

        }

        [Fact]
        public async Task EditarCategoriaAsync_DeveDarErro_AdminMasLivroNaoEncontrado()
        {
            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockRepoCategoria.Setup(x => x.ObterPorIdAsync(It.IsAny<int>(), true)).ReturnsAsync((Categoria?)null);

            var result = await _categoriaService.editarAsync(1, new CategoriaUpdateDTO());

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NotFound);
        }

        [Fact]
        public async Task EditarCategoriaAsync_DeveDarErro_AdminMasDadosInvalidosDeCategoria()
        {
            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            var result = await _categoriaService.editarAsync(1,null);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.Invalido);

        }

        [Fact]
        public async Task EditarCategoriaAsync_DeveDarErro_QuandoJaExisteCategoriaComMesmoNome()
        {
            var dto = new CategoriaUpdateDTO { Nome = "Teste" };

            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            var categoria = new Categoria("Outro Nome", "UserId");

            _mockRepoCategoria
                .Setup(x => x.ObterPorIdAsync(It.IsAny<int>(), true))
                .ReturnsAsync(categoria);

            _mockRepoCategoria
                .Setup(x => x.ExisteCategoriaComEsseNomeAsync(It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(true);

            var result = await _categoriaService.editarAsync(1, dto);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.Conflito);

            _mockRepoCategoria.Verify(x => x.SalvarAsync(), Times.Never);
        }
    }
}
