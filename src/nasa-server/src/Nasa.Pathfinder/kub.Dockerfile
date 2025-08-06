FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /api
COPY ["api/pathfinder.proto", "pathfinder.proto"]
COPY ["api/messages.proto", "messages.proto"]

WORKDIR /src
COPY ["src/nasa-server/src/Nasa.Pathfinder/", "nasa-server/src/Nasa.Pathfinder/"]
COPY ["src/nasa-server/src/Nasa.Pathfinder.Facades/", "nasa-server/src/Nasa.Pathfinder.Facades/"]
COPY ["src/nasa-server/src/Nasa.Pathfinder.Facades.Contracts/", "nasa-server/src/Nasa.Pathfinder.Facades.Contracts/"]
COPY ["src/nasa-server/src/Nasa.Pathfinder.Domain/", "nasa-server/src/Nasa.Pathfinder.Domain/"]
COPY ["src/nasa-server/src/Nasa.Pathfinder.Data/", "nasa-server/src/Nasa.Pathfinder.Data/"]
COPY ["src/nasa-server/src/Nasa.Pathfinder.Data.Contracts/", "nasa-server/src/Nasa.Pathfinder.Data.Contracts/"]
COPY ["src/nasa-server/src/Nasa.Pathfinder.Infrastructure/", "nasa-server/src/Nasa.Pathfinder.Infrastructure/"]
COPY ["src/nasa-server/src/Nasa.Pathfinder.Infrastructure.Contracts/", "nasa-server/src/Nasa.Pathfinder.Infrastructure.Contracts/"]
COPY ["src/nasa-server/src/Nasa.Pathfinder.Services/", "nasa-server/src/Nasa.Pathfinder.Services/"]
COPY ["src/nasa-server/src/Nasa.Pathfinder.Services.Contracts/", "nasa-server/src/Nasa.Pathfinder.Services.Contracts/"]
COPY ["src/nasa-server/src/Nasa.Pathfinder.Services.Consumers/", "nasa-server/src/Nasa.Pathfinder.Services.Consumers/"]

RUN dotnet restore "nasa-server/src/Nasa.Pathfinder/Nasa.Pathfinder.csproj"
COPY . .

WORKDIR "/src/nasa-server/src/Nasa.Pathfinder"
RUN dotnet build "./Nasa.Pathfinder.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Nasa.Pathfinder.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Nasa.Pathfinder.dll"]