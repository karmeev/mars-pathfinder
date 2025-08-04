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
COPY ["src/nasa-server/Nasa.Pathfinder/", "nasa-server/Nasa.Pathfinder/"]
COPY ["src/nasa-server/Nasa.Pathfinder.Domain/", "nasa-server/Nasa.Pathfinder.Domain/"]
COPY ["src/nasa-server/Nasa.Pathfinder.Data/", "nasa-server/Nasa.Pathfinder.Data/"]
COPY ["src/nasa-server/Nasa.Pathfinder.Facades.Contracts/", "nasa-server/Nasa.Pathfinder.Facades.Contracts/"]
COPY ["src/nasa-server/Nasa.Pathfinder.Infrastructure/", "nasa-server/Nasa.Pathfinder.Infrastructure/"]
COPY ["src/nasa-server/Nasa.Pathfinder.Services/", "nasa-server/Nasa.Pathfinder.Services/"]

RUN dotnet restore "nasa-server/Nasa.Pathfinder/Nasa.Pathfinder.csproj"
COPY . .

WORKDIR "/src/nasa-server/Nasa.Pathfinder"
RUN dotnet build "./Nasa.Pathfinder.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Nasa.Pathfinder.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Nasa.Pathfinder.dll"]
