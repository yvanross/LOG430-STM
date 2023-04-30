#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Nuget/Ambassador.1.1.0.nupkg", "Ingress/"]
COPY ["Ingress/Ingress/nuget.config", ""]
COPY ["Ingress/Ingress/Ingress.csproj", "Ingress/"]
COPY ["Ingress/ApplicationLogic/ApplicationLogic.csproj", "ApplicationLogic/"]
COPY ["Ingress/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Ingress/Configuration/Configuration.csproj", "Configuration/"]
COPY ["Ingress/Entities/Entities.csproj", "Entities/"]
RUN dotnet restore "Ingress/Ingress.csproj"
COPY . .
WORKDIR "/src/Ingress"
RUN dotnet build "Configuration/Configuration.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "Configuration/Configuration.csproj" -c Debug -o /app/publish /p:UseAppHost=false

ENV ASPNETCORE_ENVIRONMENT=Development

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Configuration.dll"]