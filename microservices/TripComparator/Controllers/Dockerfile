#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Controllers/Nuget/*", ""]
COPY ["Controllers/nuget.config", ""]
COPY ["Controllers/Controllers.csproj", "Controllers/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Configuration/Configuration.csproj", "Configuration/"]
RUN dotnet restore "Controllers/Controllers.csproj"
COPY . .
WORKDIR "/src/Configuration"
RUN dotnet build "Configuration.csproj" -c Debug -o /app/build  #-c Release -o /app/build Commenting this to help debugging your container, but it is less performant

FROM build AS publish
RUN dotnet publish "Configuration.csproj" -c Debug -o /app/publish /p:UseAppHost=false #-c Release -o /app/publish /p:UseAppHost=false Commenting this to help debugging your container, but it is less performant

#here again to help debugging your container, but it might be less performant
ENV ASPNETCORE_ENVIRONMENT=Development 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Configuration.dll"]