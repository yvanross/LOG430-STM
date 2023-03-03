#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Nuget/Ambassador.*.nupkg", "TripComparator/"]
COPY ["TripComparator/TripComparator/nuget.config", ""]
COPY ["TripComparator/TripComparator/TripComparator.csproj", "TripComparator/"]
COPY ["TripComparator/ApplicationLogic/ApplicationLogic.csproj", "ApplicationLogic/"]
COPY ["TripComparator/Entities/Entities.csproj", "Entities/"]
RUN dotnet restore "TripComparator/TripComparator.csproj"
COPY . .
WORKDIR "/src/TripComparator"
RUN dotnet build "TripComparator/TripComparator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TripComparator/TripComparator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TripComparator.dll"]