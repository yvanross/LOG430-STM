#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Configuration/Configuration.csproj", "Configuration/"]
COPY ["Application.Commands/Application.Commands.csproj", "Application.Commands/"]
COPY ["Application.CommandServices/Application.CommandServices.csproj", "Application.CommandServices/"]
COPY ["Application.Dtos/Application.Dtos.csproj", "Application.Dtos/"]
COPY ["Application.Mapping/Application.Mapping.csproj", "Application.Mapping/"]
COPY ["Domain.Aggregates.Stop/Domain.Aggregates.Stop.csproj", "Domain.Aggregates.Stop/"]
COPY ["Domain.Common/Domain.Common.csproj", "Domain.Common/"]
COPY ["Domain.ValueObjects/Domain.ValueObjects.csproj", "Domain.ValueObjects/"]
COPY ["Domain.Factories/Domain.Factories.csproj", "Domain.Factories/"]
COPY ["Domain.Aggregates.Bus/Domain.Aggregates.Bus.csproj", "Domain.Aggregates.Bus/"]
COPY ["Domain.Aggregates.Ride/Domain.Aggregates.Ride.csproj", "Domain.Aggregates.Ride/"]
COPY ["Domain.Aggregates.Trip/Domain.Aggregates.Trip.csproj", "Domain.Aggregates.Trip/"]
COPY ["Application.EventHandlers/Application.EventHandlers.csproj", "Application.EventHandlers/"]
COPY ["Application.Common/Application.Common.csproj", "Application.Common/"]
COPY ["Domain.Services/Domain.Services.csproj", "Domain.Services/"]
COPY ["Application.Queries/Application.Queries.csproj", "Application.Queries/"]
COPY ["Application.QueryServices/Application.QueryServices.csproj", "Application.QueryServices/"]
COPY ["Application.ViewModels/Application.ViewModels.csproj", "Application.ViewModels/"]
COPY ["Controllers.Jobs/Controllers.Jobs.csproj", "Controllers.Jobs/"]
COPY ["Controllers.Rest/Controllers.Rest.csproj", "Controllers.Rest/"]
COPY ["Infrastructure.ApiClients/Infrastructure.ApiClients.csproj", "Infrastructure.ApiClients/"]
COPY ["Infrastructure.Events/Infrastructure.Events.csproj", "Infrastructure.Events/"]
COPY ["Infrastructure.FileHandlers/Infrastructure.FileHandlers.csproj", "Infrastructure.FileHandlers/"]
COPY ["Infrastructure.ReadRepositories/Infrastructure.ReadRepositories.csproj", "Infrastructure.ReadRepositories/"]
COPY ["Infrastructure.WriteRepositories/Infrastructure.WriteRepositories.csproj", "Infrastructure.WriteRepositories/"]
RUN dotnet restore "./Configuration/./Configuration.csproj"
COPY . .
WORKDIR "/src/Configuration"
RUN dotnet build "./Configuration.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Configuration.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Configuration.dll"]