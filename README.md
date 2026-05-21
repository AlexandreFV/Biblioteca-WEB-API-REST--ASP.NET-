# Biblioteca WEB API REST (ASP.NET)

API REST desenvolvida para aprimoramento prático no desenvolvimento backend utilizando ASP.NET Core, aplicando conceitos modernos de arquitetura, autenticação, testes automatizados e boas práticas utilizadas no mercado.

O projeto foi construído utilizando os princípios de Clean Architecture, com foco em separação de responsabilidades, organização em camadas, escalabilidade e manutenção facilitada.

# Tecnologias Utilizadas
- ASP.NET Core Web API
- C#
- Entity Framework Core
- PostgreSQL
- ASP.NET Identity
- JWT Authentication
- AutoMapper
- Docker
- xUnit
- Moq
- Swagger

# Arquitetura
O projeto foi estruturado utilizando Clean Architecture, dividido nas seguintes camadas:

src/
 ├── Domain
 ├── Application
 ├── Infrastructure
 └── API

# Objetivos da arquitetura:
- Separação de responsabilidades
- Desacoplamento entre camadas
- Facilidade de manutenção
- Escalabilidade
- Melhor testabilidade

# Funcionalidades
- Autenticação e autorização com JWT
- Cadastro e gerenciamento de livros
- Cadastro de categorias
- Solicitações de empréstimo
- Paginação de resultados
- Tratamento global de exceções
- Rate Limiting
- Testes unitários e de integração
- Documentação da API com Swagger

# Banco de Dados
O projeto utiliza PostgreSQL juntamente com Entity Framework Core para persistência de dados.

Durante os testes automatizados, é utilizado banco em memória (InMemoryDatabase) para isolamento dos testes.

# Configuração do Projeto
Pré-requisitos
- .NET 8 SDK
- PostgreSQL
- Docker (opcional)

# Configuração do User Secrets
O projeto utiliza User Secrets para armazenamento seguro da chave JWT em ambiente de desenvolvimento.

Inicialize o User Secrets
- dotnet user-secrets init
- Configure a chave JWT
- dotnet user-secrets set "Jwt:Secret" "sua_chave_super_secreta"

# Configuração do Banco de Dados
Configure a connection string no arquivo appsettings.json:

"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=BibliotecaDB;Username=postgres;Password=senha"
}

# Executando as Migrations
dotnet ef database update

# Executando o Projeto
dotnet run

# Swagger
Após executar a aplicação, acesse:

https://localhost:xxxx/swagger

# Testes
O projeto possui:
- Testes unitários
- Testes de integração
- Validação de regras de negócio
- Mocks utilizando Moq

Para executar os testes:

dotnet test
