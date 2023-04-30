#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["AuthService/AuthService/AuthService.csproj", "AuthService/"]
COPY ["AuthService/ApplicationLogic/ApplicationLogic.csproj", "ApplicationLogic/"]
COPY ["AuthService/Entities/Entities.csproj", "Entities/"]
RUN dotnet restore "AuthService/AuthService.csproj"
COPY . .
WORKDIR "/src/AuthService"
RUN dotnet build "AuthService/AuthService.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "AuthService/AuthService.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthService.dll"]