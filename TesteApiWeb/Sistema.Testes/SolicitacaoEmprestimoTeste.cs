using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Models;
using Biblioteca_WEB_API_REST_ASP.Services;
using BibliotecaWebApiRest.Repositories.Interfaces;
using DTOS.SolicitacaoEmprestimo;
using FluentAssertions;
using Moq;
using Sistema.Application.Interfaces;
using Sistema.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DTOS.SolicitacaoEmprestimo.SolicitacaoEmprestimoDTO;
namespace Sistema.Testes
{
    public class SolicitacaoEmprestimoTeste
    {

        private readonly SolicitacaoEmprestimoService _solicitacaoService;

        private readonly Mock<ISolicitacaoEmprestimoRepository> _repoSolicitacaoEmprestimo;
        private readonly Mock<ICurrentUser> _currentUser;
        private readonly Mock<ILivroRepository> _livroRepository;
        private readonly Mock<IIdentityService> _identityService;

        private readonly SolicitacaoEmprestimo _solicitacaoTesteModel;

        private readonly SolicitacaoEmprestimo _solicitacaoDesativadaTesteModel;

        private readonly Usuario _usuarioTeste;
        private readonly Usuario _usuarioInativoTeste;
        private readonly Livro _livroTeste;


        public SolicitacaoEmprestimoTeste()
        {

            _repoSolicitacaoEmprestimo = new Mock<ISolicitacaoEmprestimoRepository>();
            _currentUser = new Mock<ICurrentUser>();
            _livroRepository = new Mock<ILivroRepository>();
            _identityService = new Mock<IIdentityService>();

            _solicitacaoService = new SolicitacaoEmprestimoService(
                _repoSolicitacaoEmprestimo.Object,
                _currentUser.Object,
                _livroRepository.Object,
                _identityService.Object
                );

            _solicitacaoTesteModel = new SolicitacaoEmprestimo
            {
                Ativo = true,
                DataSolicitacao = DateTime.UtcNow,
                Id = 1, 
                DataAlteracaoStatus = DateTime.UtcNow,
                IdLivro = 1,
                IdUsuarioAdmin = "Admin teste",
                IdUsuarioCliente = "Cliente teste",
                Status = StatusSolicitacao.Aguardando,
                
            };

            _solicitacaoDesativadaTesteModel = new SolicitacaoEmprestimo
            {
                Ativo = false,
                DataSolicitacao = DateTime.UtcNow,
                Id = 1,
                DataAlteracaoStatus = DateTime.UtcNow,
                IdLivro = 1,
                IdUsuarioAdmin = "Admin teste",
                IdUsuarioCliente = "Cliente teste",
                Status = StatusSolicitacao.Aguardando,

            };

            _usuarioTeste = new Usuario
            {
                Ativo = true,
                Email = "AlexandreTeste@123.com",
                Id = "Cliente teste",
                Nome = "Alexandre Teste",
                UserName = "UserTeste",

            };

            _usuarioInativoTeste = new Usuario
            {
                Ativo = false,
                Email = "AlexandreTeste@123.com",
                Id = "Cliente teste",
                Nome = "Alexandre Teste",
                UserName = "UserTeste",

            };

            _livroTeste = new Livro
            {
                Ativo = true,
                DataCriacao = DateTime.UtcNow,
                Id = 1,
                IdUsuarioAdminCriou = "Admin teste",
                Nome = "Livro Teste",
                Quantidade = 10,
                
            };
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task BuscarPorId_DeveDarSucesso_IdValidoClienteLogado(bool isAdmin)
        {
            
            _currentUser.Setup(x => x.IsAdmin).Returns(isAdmin);
            _currentUser.Setup(x => x.userId).Returns("Cliente teste");

            _repoSolicitacaoEmprestimo.Setup(x => x.ObterSolicitacaoEmprestimoPorId(It.IsAny<int>(), "Cliente teste", isAdmin, isAdmin)).ReturnsAsync(_solicitacaoTesteModel);

            var result = await _solicitacaoService.buscarPorId(1);

            result.Sucesso.Should().BeTrue();
            result.Tipo.Should().Be(ResultType.Sucesso);

            _repoSolicitacaoEmprestimo.Verify(x => x.ObterSolicitacaoEmprestimoPorId(It.IsAny<int>(), "Cliente teste", isAdmin, isAdmin), Times.Once);

        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task BuscarPorId_DeveDarErro_SolicitacaoNaoEncontrada(bool isAdmin)
        {

            _currentUser.Setup(x => x.IsAdmin).Returns(isAdmin);
            _currentUser.Setup(x => x.userId).Returns("Cliente teste");

            _repoSolicitacaoEmprestimo.Setup(x => x.ObterSolicitacaoEmprestimoPorId(It.IsAny<int>(), "Cliente teste", isAdmin, isAdmin)).ReturnsAsync((SolicitacaoEmprestimo?)null);

            var result = await _solicitacaoService.buscarPorId(1);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NotFound);

            _repoSolicitacaoEmprestimo.Verify(x => x.ObterSolicitacaoEmprestimoPorId(It.IsAny<int>(), "Cliente teste", isAdmin, isAdmin), Times.Once);

        }

        [Fact]
        public async Task BuscarPorId_DeveDarErro_SolicitacaoExisteMasSemPermissao()
        {

            _currentUser.Setup(x => x.IsAdmin).Returns(false);
            _currentUser.Setup(x => x.userId).Returns("Cliente teste");

            _repoSolicitacaoEmprestimo.Setup(x => x.ObterSolicitacaoEmprestimoPorId(It.IsAny<int>(), "Cliente teste", false, false)).ReturnsAsync(_solicitacaoDesativadaTesteModel);

            var result = await _solicitacaoService.buscarPorId(1);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NotFound);

            _repoSolicitacaoEmprestimo.Verify(x => x.ObterSolicitacaoEmprestimoPorId(It.IsAny<int>(), "Cliente teste", false, false), Times.Once);

        }

        [Fact]
        public async Task SoftDelete_DeveDarSucesso_AdminLogado()
        {

            _currentUser.Setup(x => x.IsAdmin).Returns(true);
            _currentUser.Setup(x => x.userId).Returns("Cliente teste");

            _repoSolicitacaoEmprestimo.Setup(x => x.ObterSolicitacaoEmprestimoPorId(It.IsAny<int>(), "Cliente teste", true, true)).ReturnsAsync(_solicitacaoDesativadaTesteModel);

            var result = await _solicitacaoService.softDeletePorId(1);

            result.Sucesso.Should().BeTrue();
            result.Tipo.Should().Be(ResultType.Sucesso);

            _repoSolicitacaoEmprestimo.Verify(x => x.ObterSolicitacaoEmprestimoPorId(It.IsAny<int>(), "Cliente teste", true, true), Times.Once);
            _repoSolicitacaoEmprestimo.Verify(x => x.SalvarAsync(), Times.Once);
        }

        [Fact]
        public async Task SoftDelete_DeveDarErro_ClienteLogado()
        {
            _currentUser.Setup(x => x.IsAdmin).Returns(false);
            _currentUser.Setup(x => x.userId).Returns("Cliente teste");

            var result = await _solicitacaoService.softDeletePorId(1);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NaoAutorizado);

            _repoSolicitacaoEmprestimo.Verify(x => x.ObterSolicitacaoEmprestimoPorId(It.IsAny<int>(), "Cliente teste", It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task SoftDelete_DeveDarErro_AdminLogadoMasSolicitacaoNaoEncontrada()
        {

            _currentUser.Setup(x => x.IsAdmin).Returns(true);
            _currentUser.Setup(x => x.userId).Returns("Cliente teste");

            _repoSolicitacaoEmprestimo.Setup(x => x.ObterSolicitacaoEmprestimoPorId(It.IsAny<int>(), "Cliente teste", true, true)).ReturnsAsync((SolicitacaoEmprestimo?)null);

            var result = await _solicitacaoService.softDeletePorId(1);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NotFound);

            _repoSolicitacaoEmprestimo.Verify(x => x.ObterSolicitacaoEmprestimoPorId(It.IsAny<int>(), "Cliente teste", true, true), Times.Once);
            _repoSolicitacaoEmprestimo.Verify(x => x.SalvarAsync(), Times.Never);
        }

        [Fact]
        public async Task AdicionarAsync_DeveDarSucesso_DadosValidos()
        {

            var dto = new SolicitacaoEmprestimoDTOCreate
            {
                IdLivro = 1

            };

            _currentUser.Setup(x => x.userId).Returns("Cliente teste");

            _identityService.Setup(x => x.encontrarUsuarioPorId("Cliente teste")).ReturnsAsync(_usuarioTeste);

            _livroRepository.Setup(x => x.ObterLivroIncluindoCategoriaPorId(It.IsAny<int>(), false)).ReturnsAsync(_livroTeste);

            _repoSolicitacaoEmprestimo.Setup(x => x.ExisteSolicitacaoAtivaParaUsuarioELivroAsync("Cliente teste", It.IsAny<int>())).ReturnsAsync(false);

            var result = await _solicitacaoService.adicionarAsync(dto);

            result.Sucesso.Should().BeTrue();
            result.Tipo.Should().Be(ResultType.Criado);

            _repoSolicitacaoEmprestimo.Verify(x =>
                x.AdicionarAsync(It.Is<SolicitacaoEmprestimo>(s =>
                    s.IdLivro == 1 &&
                    s.IdUsuarioCliente == "Cliente teste" &&
                    s.Status == StatusSolicitacao.Aguardando &&
                    s.Ativo == true
                )),
                Times.Once
            );
            
            _repoSolicitacaoEmprestimo.Verify(x => x.SalvarAsync(), Times.Once);

        }

        [Fact]
        public async Task AdicionarAsync_DeveDarErro_DadosInvalidos()
        {
            var result = await _solicitacaoService.adicionarAsync((SolicitacaoEmprestimoDTOCreate?)null);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.Invalido);

            _identityService.Verify(x => x.encontrarUsuarioPorId(It.IsAny<string>()), Times.Never);
            _livroRepository.Verify(x => x.ObterLivroIncluindoCategoriaPorId(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
            _repoSolicitacaoEmprestimo.Verify(x => x.ExisteSolicitacaoAtivaParaUsuarioELivroAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            _repoSolicitacaoEmprestimo.Verify(x => x.AdicionarAsync(It.IsAny<SolicitacaoEmprestimo>()), Times.Never);
            _repoSolicitacaoEmprestimo.Verify(x => x.SalvarAsync(), Times.Never);
            
        }

        [Fact]
        public async Task AdicionarAsync_DeveDarErro_UsuarioNaoEncontrado()
        {

            var dto = new SolicitacaoEmprestimoDTOCreate
            {
                IdLivro = 1

            };

            _currentUser.Setup(x => x.userId).Returns("Cliente teste");

            _identityService.Setup(x => x.encontrarUsuarioPorId("Cliente teste")).ReturnsAsync((Usuario?)null);

            var result = await _solicitacaoService.adicionarAsync(dto);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NotFound);

        }

        [Fact]
        public async Task AdicionarAsync_DeveDarErro_UsuarioInativo()
        {

            var dto = new SolicitacaoEmprestimoDTOCreate
            {
                IdLivro = 1

            };

            _currentUser.Setup(x => x.userId).Returns("Cliente teste");

            _identityService.Setup(x => x.encontrarUsuarioPorId("Cliente teste")).ReturnsAsync(_usuarioInativoTeste);

            var result = await _solicitacaoService.adicionarAsync(dto);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NotFound);

        }

        [Fact]
        public async Task AdicionarAsync_DeveDarErro_LivroNaoEncontrado()
        {

            var dto = new SolicitacaoEmprestimoDTOCreate
            {
                IdLivro = 1

            };

            _currentUser.Setup(x => x.userId).Returns("Cliente teste");

            _identityService.Setup(x => x.encontrarUsuarioPorId("Cliente teste")).ReturnsAsync(_usuarioTeste);

            _livroRepository.Setup(x => x.ObterLivroIncluindoCategoriaPorId(It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync((Livro?)null);

            var result = await _solicitacaoService.adicionarAsync(dto);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NotFound);

            _livroRepository.Verify(x => x.ObterLivroIncluindoCategoriaPorId(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
            _repoSolicitacaoEmprestimo.Verify(x => x.ExisteSolicitacaoAtivaParaUsuarioELivroAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);

            
        }

        [Fact]
        public async Task AdicionarAsync_DeveDarErro_SolicitacaoJaExiste()
        {
            var dto = new SolicitacaoEmprestimoDTOCreate
            {
                IdLivro = 1
            };

            _currentUser.Setup(x => x.userId).Returns("Cliente teste");

            _identityService.Setup(x => x.encontrarUsuarioPorId("Cliente teste"))
                .ReturnsAsync(_usuarioTeste);

            _livroRepository.Setup(x => x.ObterLivroIncluindoCategoriaPorId(1, false))
                .ReturnsAsync(_livroTeste);

            _repoSolicitacaoEmprestimo
                .Setup(x => x.ExisteSolicitacaoAtivaParaUsuarioELivroAsync("Cliente teste", 1))
                .ReturnsAsync(true);

            var result = await _solicitacaoService.adicionarAsync(dto);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.Conflito);

            _repoSolicitacaoEmprestimo.Verify(x => x.AdicionarAsync(It.IsAny<SolicitacaoEmprestimo>()), Times.Never);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ListarTodasSolicitacoes_DeveDarSucesso(bool isAdmin)
        {

            IEnumerable<SolicitacaoEmprestimo> listaItens = new List<SolicitacaoEmprestimo>();

            _currentUser.Setup(x => x.IsAdmin).Returns(isAdmin);

            _repoSolicitacaoEmprestimo.Setup(x => x.ObterTodosPorPermissaoAsync(isAdmin, isAdmin, It.IsAny<string>())).ReturnsAsync(listaItens);

            var result = await _solicitacaoService.listarAsync();

            result.Sucesso.Should().BeTrue();
            result.Tipo.Should().Be(ResultType.Sucesso);

            _repoSolicitacaoEmprestimo.Verify(x => x.ObterTodosPorPermissaoAsync(It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>()), Times.Once);


        }

        [Fact]
        public async Task ListarMinhasSolicitacoes_DeveDarSucesso()
        {

            IEnumerable<SolicitacaoEmprestimo> listaItens = new List<SolicitacaoEmprestimo>();

            _currentUser.Setup(x => x.userId).Returns("Cliente teste");

            _repoSolicitacaoEmprestimo.Setup(x => x.ObterTodasAsMinhasSolicitacoes(It.IsAny<string>())).ReturnsAsync(listaItens);

            var result = await _solicitacaoService.ListarMinhasSolicitacoesAsync();

            result.Sucesso.Should().BeTrue();
            result.Tipo.Should().Be(ResultType.Sucesso);

            _repoSolicitacaoEmprestimo.Verify(x => x.ObterTodasAsMinhasSolicitacoes(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task AlterarStatusSolicitacao_DeveDarSucesso_DadosValidos()
        {
            var dto = new SolicitacaoEmprestimoDTOUpdateAdmin
            {
                Status = StatusSolicitacao.Aprovado
            };

            _currentUser.Setup(x => x.IsAdmin).Returns(true);

            _repoSolicitacaoEmprestimo.Setup(x => x.ObterSolicitacaoEmprestimoPorId(It.IsAny<int>(), It.IsAny<string>(), true, true)).ReturnsAsync(_solicitacaoTesteModel);

            var result = await _solicitacaoService.alterarStatusDaSolicitacao(1, dto);

            result.Sucesso.Should().BeTrue();
            result.Tipo.Should().Be(ResultType.Atualizado);

            _repoSolicitacaoEmprestimo.Verify(x => x.SalvarAsync(), Times.Once);
        }


        [Fact]
        public async Task AlterarStatusSolicitacao_DeveDarErro_UsuarioNaoAdmin()
        {

            var dto = new SolicitacaoEmprestimoDTOUpdateAdmin
            {
                Status = StatusSolicitacao.Aprovado
            };

            _currentUser.Setup(x => x.IsAdmin).Returns(false);

            var result = await _solicitacaoService.alterarStatusDaSolicitacao(1, dto);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NaoAutorizado);

            _repoSolicitacaoEmprestimo.Verify(x => x.SalvarAsync(), Times.Never);
        }


        [Fact]
        public async Task AlterarStatusSolicitacao_DeveDarErro_DTOInvalidos()
        {
            _currentUser.Setup(x => x.IsAdmin).Returns(true);

            var result = await _solicitacaoService.alterarStatusDaSolicitacao(1, (SolicitacaoEmprestimoDTOUpdateAdmin?) null);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.Invalido);

            _repoSolicitacaoEmprestimo.Verify(x => x.SalvarAsync(), Times.Never);
        }

        [Fact]
        public async Task AlterarStatusSolicitacao_DeveDarErro_SolicitacaoDesejadaNaoEncontrada()
        {

            var dto = new SolicitacaoEmprestimoDTOUpdateAdmin
            {
                Status = StatusSolicitacao.Aprovado
            };

            _currentUser.Setup(x => x.IsAdmin).Returns(true);

            _repoSolicitacaoEmprestimo.Setup(x => x.ObterSolicitacaoEmprestimoPorId(It.IsAny<int>(), It.IsAny<string>(), true, true)).ReturnsAsync((SolicitacaoEmprestimo?)null);

            var result = await _solicitacaoService.alterarStatusDaSolicitacao(1, dto);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NotFound);

            _repoSolicitacaoEmprestimo.Verify(x => x.SalvarAsync(), Times.Never);
        }

        [Fact]
        public async Task AlterarStatusSolicitacao_DeveDarErro_SolicitacaoDesejadaEstaAprovada()
        {

            var dto = new SolicitacaoEmprestimoDTOUpdateAdmin
            {
                Status = StatusSolicitacao.Aprovado
            };

            _currentUser.Setup(x => x.IsAdmin).Returns(true);

            _solicitacaoTesteModel.Status = StatusSolicitacao.Aprovado;
            _repoSolicitacaoEmprestimo.Setup(x => x.ObterSolicitacaoEmprestimoPorId(It.IsAny<int>(), It.IsAny<string>(), true, true)).ReturnsAsync(_solicitacaoTesteModel);

            var result = await _solicitacaoService.alterarStatusDaSolicitacao(1, dto);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.Conflito);

            _repoSolicitacaoEmprestimo.Verify(x => x.SalvarAsync(), Times.Never);
        }


    }
}
