#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Aspect.Configuration/Aspect.Configuration.csproj", "Aspect.Configuration/"]
COPY ["Application.Commands/Application.Commands.csproj", "Application.Commands/"]
COPY ["Application.WriteServices/Application.CommandServices.csproj", "Application.WriteServices/"]
COPY ["Application.Dtos/Application.Dtos.csproj", "Application.Dtos/"]
COPY ["Domain.ValueObjects/Domain.ValueObjects.csproj", "Domain.ValueObjects/"]
COPY ["Domain.Common/Domain.Common.csproj", "Domain.Common/"]
COPY ["Domain.Events/Domain.Events.csproj", "Domain.Events/"]
COPY ["Application.EventHandlers/Application.EventHandlers.csproj", "Application.EventHandlers/"]
COPY ["Application.Mapping/Application.Mapping.csproj", "Application.Mapping/"]
COPY ["Domain.Aggregates/Domain.Aggregates.csproj", "Domain.Aggregates/"]
COPY ["Domain.Entities/Domain.Entities.csproj", "Domain.Entities/"]
COPY ["Domain.Factories/Domain.Factories.csproj", "Domain.Factories/"]
COPY ["Domain.Services/Domain.Services.csproj", "Domain.Services/"]
COPY ["Application.Common/Application.Common.csproj", "Application.Common/"]
COPY ["Application.Queries/Application.Queries.csproj", "Application.Queries/"]
COPY ["Application.ReadServices/Application.QueryServices.csproj", "Application.ReadServices/"]
COPY ["Application.ViewModels/Application.ViewModels.csproj", "Application.ViewModels/"]
COPY ["Controllers.Rest/Controllers.Rest.csproj", "Controllers.Rest/"]
COPY ["Infrastructure.ApiClients/Infrastructure.ApiClients.csproj", "Infrastructure.ApiClients/"]
COPY ["Infrastructure.FileHandlers/Infrastructure.FileHandlers.csproj", "Infrastructure.FileHandlers/"]
COPY ["Infrastructure.ReadRepositories/Infrastructure.ReadRepositories.csproj", "Infrastructure.ReadRepositories/"]
COPY ["Infrastructure.TcpClients/Infrastructure.TcpClients.csproj", "Infrastructure.TcpClients/"]
COPY ["Infrastructure.WriteRepositories/Infrastructure.WriteRepositories.csproj", "Infrastructure.WriteRepositories/"]
RUN dotnet restore "Aspect.Configuration/Aspect.Configuration.csproj"
COPY . .
WORKDIR "/src/Aspect.Configuration"
RUN dotnet build "Aspect.Configuration.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Aspect.Configuration.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Aspect.Configuration.dll"]