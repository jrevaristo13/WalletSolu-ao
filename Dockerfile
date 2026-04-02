# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copia o arquivo de solução e TODOS os arquivos .csproj das subpastas
# Isso é essencial para o 'dotnet restore' funcionar em arquitetura DDD
COPY ["WalletSolution.sln", "./"]
COPY ["Wallet.Api/Wallet.Api.csproj", "Wallet.Api/"]
COPY ["Wallet.Domain/Wallet.Domain.csproj", "Wallet.Domain/"]
COPY ["Wallet.Application/Wallet.Application.csproj", "Wallet.Application/"]
COPY ["Wallet.Infrastructure/Wallet.Infrastructure.csproj", "Wallet.Infrastructure/"]

# 2. Restaura as dependências (NuGet)
RUN dotnet restore "WalletSolution.sln"

# 3. Agora copia o restante dos arquivos (o código fonte em si)
COPY . .

# 4. Compila o projeto de API
RUN dotnet publish "Wallet.Api/Wallet.Api.csproj" -c Release -o /app/publish

# Estágio de Execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# 5. Configuração de porta dinâmica para o Render
# O .NET vai ler a variável $PORT que o Render injeta automaticamente
ENV ASPNETCORE_URLS=http://+:${PORT}

# Garante que o nome da DLL seja o correto do seu projeto
ENTRYPOINT ["dotnet", "Wallet.Api.dll"]