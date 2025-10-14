# ========= Base Image =========
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5001

# ========= Build Stage =========
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY Services/Payment/Payment.Domain/Payment.Domain.csproj Payment.Domain/
COPY Services/Payment/Payment.Application/Payment.Application.csproj Payment.Application/
COPY Services/Payment/Payment.Infrastructure/Payment.Infrastructure.csproj Payment.Infrastructure/
COPY Services/Payment/Payment.Api/Payment.Api.csproj Payment.Api/
RUN dotnet restore "Payment.Api/Payment.Api.csproj"
COPY Services/Payment/ ./
WORKDIR "/src/Payment.Api"
RUN dotnet build "Payment.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ========= Publish Stage =========
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Payment.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ========= Final Stage =========
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Payment.Api.dll"]
