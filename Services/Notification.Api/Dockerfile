FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5003
EXPOSE 8082

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Notification.Api/Notification.Api.csproj", "Notification.Api/"]
COPY ["Notification.Application/Notification.Application.csproj", "Notification.Application/"]
COPY ["Notification.Domain/Notification.Domain.csproj", "Notification.Domain/"]
COPY ["Notification.Infrastructure/Notification.Infrastructure.csproj", "Notification.Infrastructure/"]

RUN dotnet restore "Notification.Api/Notification.Api.csproj"
COPY . .
WORKDIR "/src/Notification.Api"
RUN dotnet build "Notification.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Notification.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notification.Api.dll"]
