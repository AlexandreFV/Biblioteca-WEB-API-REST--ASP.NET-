using Biblioteca_WEB_API_REST_ASP.Class;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;
using static DTOS.Auth.AuthDTO;

namespace Sistema.Testes.Integration
{
    public class AuthIntegration : IClassFixture<MyWebApplicationFactory>
    {

        private readonly HttpClient _requisicao;

        public AuthIntegration(MyWebApplicationFactory factory)
        {
            _requisicao = factory.CreateClient();
        }

        [Fact]
        public async Task CriarUsuario_DeveRetornar200()
        {
            var dto = new RegisterDTOCreate { Nome = "Usuario integracao200", Senha = "senha123", Email = "Email200" };

            var response = await _requisicao.PostAsJsonAsync("/api/auth/register?isAdmin=false", dto);
            
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await response.Content.ReadFromJsonAsync<ServiceResult<RegisterDTOResponse>>();

            content.Should().NotBeNull();
            content!.Sucesso.Should().BeTrue();
            content.Tipo.Should().Be(ResultType.Sucesso);
            content.Dados!.Nome.Should().Be("Usuario Integracao200");
        }

        [Fact]
        public async Task CriarUsuario_EmailJaExiste_DeveRetornar409()
        {
            var dto = new RegisterDTOCreate { Nome = "usuario integracao409", Senha = "senha123", Email = "Email409" };

            var response1 = await _requisicao.PostAsJsonAsync("/api/auth/register?isAdmin=false", dto);
            response1.StatusCode.Should().Be(HttpStatusCode.OK);

            var content1 = await response1.Content.ReadFromJsonAsync<ServiceResult<RegisterDTOCreate>>();
            content1.Should().NotBeNull();
            content1!.Sucesso.Should().BeTrue();
            content1.Tipo.Should().Be(ResultType.Sucesso);
            content1.Dados!.Nome.Should().Be("Usuario Integracao409");

            var response2 = await _requisicao.PostAsJsonAsync("/api/auth/register?isAdmin=false", dto);

            response2.StatusCode.Should().Be(HttpStatusCode.Conflict);

            var content2 = await response2.Content.ReadFromJsonAsync<ServiceResult<RegisterDTOCreate>>();

            content2.Should().NotBeNull();
            content2!.Sucesso.Should().BeFalse();
            content2.Tipo.Should().Be(ResultType.Conflito);

        }

        [Fact]
        public async Task CriarUsuario_DTOInvalido_DeveRetornar400()
        {
            var dto = new RegisterDTOCreate {  };

            var response1 = await _requisicao.PostAsJsonAsync("/api/auth/register?isAdmin=false", dto);
            
            response1.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var content = await response1.Content.ReadFromJsonAsync<ServiceResult<RegisterDTOCreate>>();

            content!.Dados.Should().BeNull();
            content.Tipo.Should().Be(ResultType.Invalido);

        }

        [Fact]
        public async Task EntrarUsuario_DTOInvalido_DeveRetornar400()
        {

            var dto = new LoginDTO { };

            var response = await _requisicao.PostAsJsonAsync("api/auth/login", dto);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var content = await response.Content.ReadFromJsonAsync<ServiceResult<LoginResponseDTO>>();

            content!.Should().NotBeNull();
            content.Sucesso.Should().BeFalse();
            content.Tipo.Should().Be(ResultType.Invalido);
        }

        [Fact]
        public async Task EntrarUsuario_UsuarioOuSenhaIncorretos_DeveRetornar401()
        {

            var dtoRegister = new RegisterDTOCreate { Nome = "nome login401", Email = "emailLogin401", Senha = "senha login401" };

            var responseCriacao = await _requisicao.PostAsJsonAsync("/api/auth/register?isAdmin=false", dtoRegister);
            responseCriacao.StatusCode.Should().Be(HttpStatusCode.OK);

            var contentCreate = await responseCriacao.Content.ReadFromJsonAsync<ServiceResult<RegisterDTOCreate>>();
            contentCreate!.Should().NotBeNull();
            contentCreate.Sucesso.Should().BeTrue();
            contentCreate.Tipo.Should().Be(ResultType.Sucesso);

            var dtoLogin = new LoginDTO { Email = "email login401", Senha = "senha login401" };

            var responseLogin = await _requisicao.PostAsJsonAsync("api/auth/login", dtoLogin);
            responseLogin.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var contentLogin = await responseLogin.Content.ReadFromJsonAsync<ServiceResult<LoginResponseDTO>>();
            contentLogin!.Sucesso.Should().BeFalse();
            contentLogin.Tipo.Should().Be(ResultType.NaoAutorizado);

        }

        [Fact]
        public async Task EntrarUsuario_DeveRetornar200()
        {

            var dtoRegister = new RegisterDTOCreate { Nome = "nome login200", Email = "emailLogin200", Senha = "senha login200" };

            var responseCriacao = await _requisicao.PostAsJsonAsync("/api/auth/register?isAdmin=false", dtoRegister);
            responseCriacao.StatusCode.Should().Be(HttpStatusCode.OK);

            var contentCreate = await responseCriacao.Content.ReadFromJsonAsync<ServiceResult<RegisterDTOCreate>>();
            contentCreate!.Should().NotBeNull();
            contentCreate.Sucesso.Should().BeTrue();
            contentCreate.Tipo.Should().Be(ResultType.Sucesso);

            var dtoLogin = new LoginDTO { Email = "emailLogin200", Senha = "senha login200" };

            var responseLogin = await _requisicao.PostAsJsonAsync("api/auth/login", dtoLogin);
            responseLogin.StatusCode.Should().Be(HttpStatusCode.OK);

            var contentLogin = await responseLogin.Content.ReadFromJsonAsync<ServiceResult<LoginResponseDTO>>();
            contentLogin!.Should().NotBeNull();
            contentLogin!.Sucesso.Should().BeTrue();
            contentLogin.Tipo.Should().Be(ResultType.Sucesso);

        }
    }
}
