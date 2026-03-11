using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Models;
using Biblioteca_WEB_API_REST_ASP.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Sistema.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DTOS.Auth.AuthDTO;

namespace Sistema.Testes
{
    public class AuthTeste
    {
        private readonly AuthService _authService;

        private readonly Mock<IIdentityService> _identityService;
        private readonly Mock<ILogger<AuthService>> _logger;

        private readonly Usuario _usuarioAtivo;
        private readonly Usuario _usuarioInativo;

        public AuthTeste()
        {
            _identityService = new Mock<IIdentityService>();
            _logger = new Mock<ILogger<AuthService>>();

            _authService = new AuthService(
                _identityService.Object,
                _logger.Object
            );

            _usuarioAtivo = new Usuario
            {
                Id = "1",
                Nome = "Alexandre",
                Email = "teste@email.com",
                Ativo = true
            };

            _usuarioInativo = new Usuario
            {
                Id = "2",
                Nome = "Alexandre",
                Email = "teste@email.com",
                Ativo = false
            };
        }


        [Fact]
        public async Task CriarUsuarioAsync_DeveDarSucess_CriarUsuarioComSucesso()
        {
            var dto = new RegisterDTOCreate
            {
                Nome = "Alex",
                Email = "teste@email.com",
                Senha = "123456"
            };

            _identityService
                .Setup(x => x.encontrarUsuarioPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((Usuario?)null);

            _identityService
                .Setup(x => x.criarAsync(It.IsAny<Usuario>(), dto.Senha))
                .ReturnsAsync((true, Enumerable.Empty<string>()));

            _identityService
                .Setup(x => x.adicionarRoleAsync(It.IsAny<Usuario>(), "Client"))
                .ReturnsAsync(true);

            var result = await _authService.CriarUsuarioAsync(dto, false);

            result.Sucesso.Should().BeTrue();
            result.Tipo.Should().Be(ResultType.Sucesso);
        }

        [Fact]
        public async Task CriarUsuarioAsync_DeveDarErro_EmailJaExiste()
        {
            var dto = new RegisterDTOCreate
            {
                Nome = "Alex",
                Email = "teste@email.com",
                Senha = "123456"
            };

            _identityService
                .Setup(x => x.encontrarUsuarioPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(_usuarioAtivo);

            var result = await _authService.CriarUsuarioAsync(dto, false);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.Conflito);
        }

        [Fact]
        public async Task CriarUsuarioAsync_DeveDarErro_FalhaCriacaoUsuario()
        {
            var dto = new RegisterDTOCreate
            {
                Nome = "Alex",
                Email = "teste@email.com",
                Senha = "123456"
            };

            _identityService
                .Setup(x => x.encontrarUsuarioPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((Usuario?)null);

            _identityService
                .Setup(x => x.criarAsync(It.IsAny<Usuario>(), dto.Senha))
                .ReturnsAsync((false, new List<string> { "Erro criação" }));

            var result = await _authService.CriarUsuarioAsync(dto, false);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.Erro);
        }

        [Fact]
        public async Task CriarUsuarioAsync_DeveDarErro_FalhaAdicionarRole()
        {
            var dto = new RegisterDTOCreate
            {
                Nome = "Alex",
                Email = "teste@email.com",
                Senha = "123456"
            };

            _identityService
                .Setup(x => x.encontrarUsuarioPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((Usuario?)null);

            _identityService
                .Setup(x => x.criarAsync(It.IsAny<Usuario>(), dto.Senha))
                .ReturnsAsync((true, Enumerable.Empty<string>()));

            _identityService
                .Setup(x => x.adicionarRoleAsync(It.IsAny<Usuario>(), "Client"))
                .ReturnsAsync(false);

            var result = await _authService.CriarUsuarioAsync(dto, false);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.Erro);
        }

        [Fact]
        public async Task EntrarAsync_DeveDarSucesso_LogarUsuarioComSucesso()
        {
            var dto = new LoginDTO
            {
                Email = "teste@email.com",
                Senha = "123456"
            };

            _identityService
                .Setup(x => x.encontrarUsuarioPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(_usuarioAtivo);

            _identityService
                .Setup(x => x.confirmarSenhaAsync(_usuarioAtivo, dto.Senha))
                .ReturnsAsync(true);

            _identityService
                .Setup(x => x.obterToken(_usuarioAtivo))
                .ReturnsAsync("tokenFake");

            var result = await _authService.EntrarAsync(dto);

            result.Sucesso.Should().BeTrue();
            result.Tipo.Should().Be(ResultType.Sucesso);
        }

        [Fact]
        public async Task EntrarAsync_DeveDarErro_UsuarioNaoEncontrado()
        {
            var dto = new LoginDTO
            {
                Email = "teste@email.com",
                Senha = "123"
            };

            _identityService
                .Setup(x => x.encontrarUsuarioPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((Usuario?)null);

            var result = await _authService.EntrarAsync(dto);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NaoAutorizado);
        }

        [Fact]
        public async Task EntrarAsync_DeveDarErro_UsuarioInativo()
        {
            var dto = new LoginDTO
            {
                Email = "teste@email.com",
                Senha = "123"
            };

            _identityService
                .Setup(x => x.encontrarUsuarioPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(_usuarioInativo);

            var result = await _authService.EntrarAsync(dto);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NaoAutorizado);
        }

        [Fact]
        public async Task EntrarAsync_DeveDarErro_SenhaInvalida()
        {
            var dto = new LoginDTO
            {
                Email = "teste@email.com",
                Senha = "123"
            };

            _identityService
                .Setup(x => x.encontrarUsuarioPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(_usuarioAtivo);

            _identityService
                .Setup(x => x.confirmarSenhaAsync(_usuarioAtivo, dto.Senha))
                .ReturnsAsync(false);

            var result = await _authService.EntrarAsync(dto);

            result.Sucesso.Should().BeFalse();
            result.Tipo.Should().Be(ResultType.NaoAutorizado);
        }
    }
}
