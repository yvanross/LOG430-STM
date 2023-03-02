#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Nuget/Ambassador.1.1.0.nupkg", "Monitor/"]
COPY ["Monitor/Monitor/nuget.config", ""]
COPY ["Monitor/Monitor/Monitor.csproj", "Monitor/"]
COPY ["Monitor/ApplicationLogic/ApplicationLogic.csproj", "ApplicationLogic/"]
COPY ["Monitor/Entities/Entities.csproj", "Entities/"]
RUN dotnet restore "Monitor/Monitor.csproj"
COPY . .
WORKDIR "/src/Monitor"
RUN dotnet build "Monitor/Monitor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Monitor/Monitor.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Monitor.dll"]