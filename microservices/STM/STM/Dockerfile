#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Nuget/Ambassador.*.nupkg", "STM/"]
COPY ["STM/STM/nuget.config", ""]
COPY ["STM/STM/STM.csproj", "STM/"]
COPY ["STM/ApplicationLogic/ApplicationLogic.csproj", "ApplicationLogic/"]
COPY ["STM/StaticGTFS/GTFS.csproj", "GTFS/"]
COPY ["STM/Entities/Entities.csproj", "Entities/"]
RUN dotnet restore "STM/STM.csproj"
COPY . .
WORKDIR "/src/STM"
RUN dotnet build "STM/STM.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "STM/STM.csproj" -c Debug -o /app/publish /p:UseAppHost=false

ENV ASPNETCORE_ENVIRONMENT=Development

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "STM.dll"]