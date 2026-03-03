using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Models;
using Biblioteca_WEB_API_REST_ASP.Services;
using BibliotecaWebApiRest.Repositories.Interfaces;
using DTOS.Categoria;
using Moq;
using Sistema.Application.Interfaces;
using Sistema.Application.Interfaces.Services;
using static DTOS.Categoria.CategoriaDTO;

namespace Sistema.Testes
{
    public class CategoriaTeste
    {

        private readonly CategoriaService _categoriaService;
        private readonly Mock<ICurrentUser> _mockUser;
        private readonly Mock<ICategoriaRepository> _mockRepoCategoria;

        public CategoriaTeste() 
        {
            _mockUser = new Mock<ICurrentUser>();
            _mockRepoCategoria = new Mock<ICategoriaRepository>();

            _categoriaService = new CategoriaService(
                _mockRepoCategoria.Object,
                _mockUser.Object
                );
        }

        [Fact]
        public async Task AdicionarCategoriaAsync_DeveCriarCategoria_QuandoDadosValidos()
        {

            //Arrange 
            var dto = new CategoriaCreateDTO()
            {
                Nome = "Categoria Teste"
            };

            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockRepoCategoria.Setup(x => x.ExisteCategoriaComEsseNomeAsync(It.IsAny<string>(), null)).ReturnsAsync(false);

            _mockRepoCategoria.Setup(x => x.AdicionarAsync(It.IsAny<Categoria>())).Returns(Task.CompletedTask);

            _mockRepoCategoria.Setup(x => x.SalvarAsync()).Returns(Task.CompletedTask);

            //Action
            var result = await _categoriaService.adicionarAsync(dto);

            //Assert
            Assert.True(result.Sucesso);

            _mockRepoCategoria.Verify(x => x.AdicionarAsync(It.IsAny<Categoria>()),Times.Once());

            _mockRepoCategoria.Verify(x => x.SalvarAsync(), Times.Once());
        }

        [Theory]
        [InlineData(false, "Teste")]
        [InlineData(true, null)]
        [InlineData(false, null)]
        public async Task AdicionarCategoriaAsync_DeveDarErro_QuandoNaoAdminOuCategoriaNula(bool isAdmin, string nomeCategoria)
        {
            var dto = nomeCategoria == null ? null : new CategoriaCreateDTO { Nome = nomeCategoria };

            _mockUser.Setup(x => x.IsAdmin).Returns(isAdmin);

            var result = await _categoriaService.adicionarAsync(dto);

            Assert.False(result.Sucesso);

        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task ListarTodasCategoriasAsync_DeverlistarSucesso_QuandoDadosValidos(bool isAdmin)
        {

            var listaApenasParaChecagem = new List<Categoria>();

            _mockUser.Setup(x => x.IsAdmin).Returns(isAdmin);

            _mockRepoCategoria.Setup(x => x.ObterTodosAsync(isAdmin)).ReturnsAsync(listaApenasParaChecagem);

            var result = await _categoriaService.listarAsync();

            _mockRepoCategoria.Verify(x => x.ObterTodosAsync(isAdmin), Times.Once());
        }

        [Fact]
        public async Task SoftDeleteCategoriaAsync_DeveApagarSucesso_ComPermissao()
        {
            var categoriaProcurada = new Categoria("Teste", "adminId");

            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockRepoCategoria.Setup(x => x.ObterPorIdAsync(It.IsAny<int>(), true)).ReturnsAsync(categoriaProcurada);

            _mockRepoCategoria.Setup(x => x.SalvarAsync()).Returns(Task.CompletedTask);

            var result = await _categoriaService.softDeletePorId(1);

            Assert.True(result.Sucesso);

            _mockRepoCategoria.Verify(x => x.SalvarAsync(), Times.Once());
        }

        [Fact]
        public async Task SoftDeleteCategoriaAsync_DeveRetornarErro_QuandoNaoAdmin()
        {
            var categoriaProcurada = new Categoria("Teste", "adminId");

            _mockUser.Setup(x => x.IsAdmin).Returns(false);

            var resul = await _categoriaService.softDeletePorId(1);

            _mockRepoCategoria
                .Verify(x => x.ObterPorIdAsync(It.IsAny<int>(), It.IsAny<bool>()),
                Times.Never);

        }

        [Fact]
        public async Task SoftDeleteCategoriaAsync_DeveRetornarErro_CategoriaNaoEncontrada()
        {
            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockRepoCategoria.Setup(x => x.ObterPorIdAsync(It.IsAny<int>(), true)).ReturnsAsync((Categoria?)null);

            var result = await _categoriaService.softDeletePorId(1);

            Assert.False(result.Sucesso);
            Assert.Equal(ResultType.NotFound, result.Tipo);

        }

        [Fact]
        public async Task SoftDeleteCategoriaAsync_DeveRetornarErro_CategoriaVinculadaLivro()
        {

            var categoria = new Categoria("Teste", "Teste");

            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockRepoCategoria.Setup(x => x.ObterPorIdAsync(It.IsAny<int>(), true)).ReturnsAsync(categoria);
            
            _mockRepoCategoria.Setup(x => x.ExisteCategoriaVinculadaAoLivroAsync(1)).ReturnsAsync(true);

            var result = await _categoriaService.softDeletePorId(1);

            Assert.False(result.Sucesso);
            Assert.Equal(ResultType.Conflito, result.Tipo);
        }

        [Fact]
        public async Task EditarCategoriaAsync_DeveEditar_DadosValidos()
        {
            var novaCategoriaEditada = new CategoriaUpdateDTO()
            {
                Nome = "Teste Atualizado",
            };

            var categoriaEntity = new Categoria("Teste Atualizado", "UserId");

            _mockUser.Setup(x => x.IsAdmin).Returns(true);

            _mockRepoCategoria.Setup(x => x.ObterPorIdAsync(It.IsAny<int>(), true)).ReturnsAsync(categoriaEntity);

            _mockRepoCategoria.Setup(x => x.ExisteCategoriaComEsseNomeAsync(novaCategoriaEditada.Nome, 1)).ReturnsAsync(false);

            _mockRepoCategoria.Setup(x => x.SalvarAsync()).Returns(Task.CompletedTask);

            var result = await _categoriaService.editarAsync(1, novaCategoriaEditada);

            Assert.True(result.Sucesso);

            _mockRepoCategoria.Verify(x => x.SalvarAsync(),Times.Once());

        }
    }
}
