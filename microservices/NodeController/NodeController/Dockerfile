#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NodeController/Entities/Entities.csproj", "Entities/"]
COPY ["NodeController/NodeController/nuget.config", ""]
COPY ["NodeController/ApplicationLogic/ApplicationLogic.csproj", "ApplicationLogic/"]
COPY ["NodeController/NodeController/NodeController.csproj", "NodeController/"]
COPY ["NodeController/Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["NodeController/Configuration/Configuration.csproj", "Configuration/"]
RUN dotnet restore "Configuration/Configuration.csproj"
COPY . .
WORKDIR "/src/NodeController"
RUN dotnet build "Configuration/Configuration.csproj" -c Debug -o /app/build #-c Release -o /app/build  #-c Debug -o /app/build ##

FROM build AS publish
RUN dotnet publish "Configuration/Configuration.csproj" -c Debug -o /app/publish /p:UseAppHost=false #-c Release -o /app/publish /p:UseAppHost=false  # -c Debug -o /app/publish /p:UseAppHost=false ##

ENV ASPNETCORE_ENVIRONMENT=Development

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Configuration.dll"]