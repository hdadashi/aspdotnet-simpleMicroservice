FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY Services/Gateway/Gateway.Domain/Gateway.Domain.csproj Gateway.Domain/
COPY Services/Gateway/Gateway.Application/Gateway.Application.csproj Gateway.Application/
COPY Services/Gateway/Gateway.Infrastructure/Gateway.Infrastructure.csproj Gateway.Infrastructure/
COPY Services/Gateway/Gateway.Api/Gateway.Api.csproj Gateway.Api/
RUN dotnet restore "Gateway.Api/Gateway.Api.csproj"
COPY Services/Gateway/ ./
WORKDIR "/src/Gateway.Api"
RUN dotnet build "Gateway.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Gateway.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gateway.Api.dll"]
