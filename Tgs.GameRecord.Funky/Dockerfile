#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Tgs.GameRecord.Funky/Tgs.GameRecord.Funky.csproj", "Tgs.GameRecord.Funky/"]
COPY ["Tgs.GameRecord.Funky.Common/Tgs.GameRecord.Funky.Common.csproj", "Tgs.GameRecord.Funky.Common/"]
COPY ["Tgs.GameRecord.Funky.Core/Tgs.GameRecord.Funky.Core.csproj", "Tgs.GameRecord.Funky.Core/"]
RUN dotnet restore "./Tgs.GameRecord.Funky/./Tgs.GameRecord.Funky.csproj"
COPY . .
WORKDIR "/src/Tgs.GameRecord.Funky"
RUN dotnet build "./Tgs.GameRecord.Funky.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Tgs.GameRecord.Funky.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tgs.GameRecord.Funky.dll"]