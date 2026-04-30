using AutoMapper;
using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Models;
using Biblioteca_WEB_API_REST_ASP.Services;
using BibliotecaWebApiRest.Repositories.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Sistema.Application.Commoms.Pagination;
using Sistema.Application.Interfaces;
using Sistema.Application.Interfaces.Services;
using static DTOS.SolicitacaoEmprestimo.SolicitacaoEmprestimoDTO;

namespace Sistema.Testes.Unit
{
    public class SolicitacaoEmprestimoTeste
    {
        private readonly SolicitacaoEmprestimoService _service;

        private readonly Mock<ISolicitacaoEmprestimoRepository> _repo;
        private readonly Mock<ICurrentUser> _currentUser;
        private readonly Mock<ILivroRepository> _livroRepo;
        private readonly Mock<IIdentityService> _identityService;
        private readonly Mock<ILogger<SolicitacaoEmprestimoService>> _logger;

        private readonly IMapper _mapper;

        private readonly SolicitacaoEmprestimo _solicitacao;
        private readonly Usuario _usuario;
        private readonly Livro _livro;

        public SolicitacaoEmprestimoTeste()
        {
            _repo = new Mock<ISolicitacaoEmprestimoRepository>();
            _currentUser = new Mock<ICurrentUser>();
            _livroRepo = new Mock<ILivroRepository>();
            _identityService = new Mock<IIdentityService>();
            _logger = new Mock<ILogger<SolicitacaoEmprestimoService>>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<SolicitacaoEmprestimo, SolicitacaoEmprestimoDTOResponse>();
            });

            _mapper = config.CreateMapper();

            _service = new SolicitacaoEmprestimoService(
                _repo.Object,
                _currentUser.Object,
                _livroRepo.Object,
                _identityService.Object,
                _logger.Object,
                _mapper
            );

            _solicitacao = new SolicitacaoEmprestimo
            {
                Id = 1,
                Ativo = true,
                IdLivro = 1,
                IdUsuarioCliente = "user",
                Status = StatusSolicitacao.Aguardando,
                DataSolicitacao = DateTime.UtcNow
            };

            _usuario = new Usuario
            {
                Id = "user",
                Ativo = true
            };

            _livro = new Livro
            {
                Id = 1,
                Ativo = true,
                Quantidade = 10
            };
        }

        [Fact]
        public async Task BuscarPorId_DeveRetornarSucesso()
        {
            _currentUser.Setup(x => x.userId).Returns("user");
            _currentUser.Setup(x => x.IsAdmin).Returns(true);

            _repo.Setup(x => x.ObterSolicitacaoEmprestimoPorId(1, "user", true, true))
                 .ReturnsAsync(_solicitacao);

            var result = await _service.buscarPorId(1);

            result.Sucesso.Should().BeTrue();
            result.Tipo.Should().Be(ResultType.Sucesso);
        }

        [Fact]
        public async Task BuscarPorId_DeveRetornarNotFound()
        {
            _currentUser.Setup(x => x.userId).Returns("user");
            _currentUser.Setup(x => x.IsAdmin).Returns(false);

            _repo.Setup(x => x.ObterSolicitacaoEmprestimoPorId(1, "user", false, false))
                 .ReturnsAsync((SolicitacaoEmprestimo?)null);

            var result = await _service.buscarPorId(1);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NotFound);
        }

        [Fact]
        public async Task Adicionar_DeveCriarComSucesso()
        {
            var dto = new SolicitacaoEmprestimoDTOCreate { IdLivro = 1 };

            _currentUser.Setup(x => x.userId).Returns("user");

            _identityService.Setup(x => x.encontrarUsuarioPorId("user"))
                .ReturnsAsync(_usuario);

            _livroRepo.Setup(x => x.ObterLivroIncluindoCategoriaPorId(1, false))
                .ReturnsAsync(_livro);

            _repo.Setup(x => x.ExisteSolicitacaoAtivaParaUsuarioELivroAsync("user", 1))
                .ReturnsAsync(false);

            var result = await _service.adicionarAsync(dto);

            result.Sucesso.Should().BeTrue();
            result.Tipo.Should().Be(ResultType.Criado);

            _repo.Verify(x => x.AdicionarAsync(It.IsAny<SolicitacaoEmprestimo>()), Times.Once);
            _repo.Verify(x => x.SalvarAsync(), Times.Once);
        }

        [Fact]
        public async Task Adicionar_DeveRetornarErro_QuandoDuplicado()
        {
            var dto = new SolicitacaoEmprestimoDTOCreate { IdLivro = 1 };

            _currentUser.Setup(x => x.userId).Returns("user");

            _identityService.Setup(x => x.encontrarUsuarioPorId("user"))
                .ReturnsAsync(_usuario);

            _livroRepo.Setup(x => x.ObterLivroIncluindoCategoriaPorId(1, false))
                .ReturnsAsync(_livro);

            _repo.Setup(x => x.ExisteSolicitacaoAtivaParaUsuarioELivroAsync("user", 1))
                .ReturnsAsync(true);

            var result = await _service.adicionarAsync(dto);

            result.Tipo.Should().Be(ResultType.Conflito);
        }

        [Fact]
        public async Task SoftDelete_DeveFuncionarParaAdmin()
        {
            _currentUser.Setup(x => x.userId).Returns("user");
            _currentUser.Setup(x => x.IsAdmin).Returns(true);

            _repo.Setup(x => x.ObterSolicitacaoEmprestimoPorId(1, "user", true, true))
                .ReturnsAsync(_solicitacao);

            var result = await _service.softDeletePorId(1);

            result.Sucesso.Should().BeTrue();

            _repo.Verify(x => x.SalvarAsync(), Times.Once);
        }

        [Fact]
        public async Task SoftDelete_DeveFalharParaNaoAdmin()
        {
            _currentUser.Setup(x => x.IsAdmin).Returns(false);

            var result = await _service.softDeletePorId(1);

            result.Tipo.Should().Be(ResultType.NaoAutorizado);
        }

        [Fact]
        public async Task AlterarStatus_DeveFuncionar()
        {
            var dto = new SolicitacaoEmprestimoDTOUpdateAdmin
            {
                Status = StatusSolicitacao.Aprovado
            };

            _currentUser.Setup(x => x.IsAdmin).Returns(true);
            _currentUser.Setup(x => x.userId).Returns("admin");

            _repo.Setup(x => x.ObterSolicitacaoEmprestimoPorId(1, "admin", true, true))
                .ReturnsAsync(_solicitacao);

            var result = await _service.alterarStatusDaSolicitacao(1, dto);

            result.Sucesso.Should().BeTrue();

            _repo.Verify(x => x.SalvarAsync(), Times.Once);
        }
    }
}