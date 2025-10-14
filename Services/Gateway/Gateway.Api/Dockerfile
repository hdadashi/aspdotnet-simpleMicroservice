# ========= Base Image =========
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5002
EXPOSE 8081

# ========= Build Stage =========
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Gateway.Api/Gateway.Api.csproj", "Gateway.Api/"]
COPY ["Gateway.Application/Gateway.Application.csproj", "Gateway.Application/"]
COPY ["Gateway.Domain/Gateway.Domain.csproj", "Gateway.Domain/"]
COPY ["Gateway.Infrastructure/Gateway.Infrastructure.csproj", "Gateway.Infrastructure/"]
RUN dotnet restore "Gateway.Api/Gateway.Api.csproj"
COPY . .
WORKDIR "/src/Gateway.Api"
RUN dotnet build "Gateway.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ========= Publish Stage =========
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Gateway.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ========= Final Stage =========
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gateway.Api.dll"]
