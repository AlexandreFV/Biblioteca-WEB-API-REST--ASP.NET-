# Acesse https://aka.ms/customizecontainer para saber como personalizar seu contêiner de depuração e como o Visual Studio usa este Dockerfile para criar suas imagens para uma depuração mais rápida.

# Esta fase é usada durante a execução no VS no modo rápido (Padrão para a configuração de Depuração)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app


ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Esta fase é usada para compilar o projeto de serviço
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Sistema.API/Sistema.API.csproj", "Sistema.API/"]
COPY ["Sistema.Application/Sistema.Application.csproj", "Sistema.Application/"]
COPY ["Sistema.Domain/Sistema.Domain.csproj", "Sistema.Domain/"]
COPY ["Sistema.Infrastructure/Sistema.Infrastructure.csproj", "Sistema.Infrastructure/"]
RUN dotnet restore "./Sistema.API/Sistema.API.csproj"
COPY . .
WORKDIR "/src/Sistema.API"
RUN dotnet build "./Sistema.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase é usada para publicar o projeto de serviço a ser copiado para a fase final
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Sistema.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase é usada na produção ou quando executada no VS no modo normal (padrão quando não está usando a configuração de Depuração)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sistema.API.dll"]