#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Nuget/ServiceMeshHelper.*.nupkg", "RouteTimeProvider/"]
COPY ["RouteTimeProvider/RouteTimeProvider/nuget.config", ""]
COPY ["RouteTimeProvider/RouteTimeProvider/RouteTimeProvider.csproj", "RouteTimeProvider/"]
COPY ["RouteTimeProvider/ApplicationLogic/ApplicationLogic.csproj", "ApplicationLogic/"]
COPY ["RouteTimeProvider/Entities/Entities.csproj", "Entities/"]
RUN dotnet restore "RouteTimeProvider/RouteTimeProvider.csproj"
COPY . .
WORKDIR "/src/RouteTimeProvider"
RUN dotnet build "RouteTimeProvider/RouteTimeProvider.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RouteTimeProvider/RouteTimeProvider.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RouteTimeProvider.dll"]	