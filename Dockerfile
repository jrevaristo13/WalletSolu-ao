FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "Wallet.Api/Wallet.Api.csproj"
RUN dotnet publish "Wallet.Api/Wallet.Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
# Comando para rodar a API na porta que o Render fornecer
CMD ASPNETCORE_URLS=http://*:$PORT dotnet Wallet.Api.dll