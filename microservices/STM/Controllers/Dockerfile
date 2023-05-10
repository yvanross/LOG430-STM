#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["STM/Entities.Common/Entities.Common.csproj", "Entities.Common/"]
COPY ["STM/Entities.Gtfs/Entities.Gtfs.csproj", "Entities.Gtfs/"]
COPY ["STM/Entities.Transit/Entities.Transit.csproj", "Entities.Transit/"]
COPY ["STM/Controllers/nuget.config", ""]
COPY ["STM/Controllers/Controllers.csproj", "Controllers/"]
COPY ["STM/ApplicationLogic/ApplicationLogic.csproj", "ApplicationLogic/"]
COPY ["STM/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["STM/Configuration/Configuration.csproj", "Configuration/"]

RUN dotnet restore "Configuration/Configuration.csproj"
COPY . .
WORKDIR "/src/STM"
RUN dotnet build "Configuration/Configuration.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "Configuration/Configuration.csproj" -c Debug -o /app/publish /p:UseAppHost=false

ENV ASPNETCORE_ENVIRONMENT=Development

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Configuration.dll"]