FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
RUN echo "Listing contents:" && ls -la
COPY ["Nasa.Pathfinder/Nasa.Pathfinder.csproj", "Nasa.Pathfinder/"]
RUN dotnet restore "Nasa.Pathfinder/Nasa.Pathfinder.csproj"
COPY . .
WORKDIR "/src/Nasa.Pathfinder"
RUN dotnet build "./Nasa.Pathfinder.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Nasa.Pathfinder.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nasa.Pathfinder.dll"]
