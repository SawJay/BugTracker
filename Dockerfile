FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["BugTracker/BugTracker.csproj", "BugTracker/"]
COPY ["BugTracker.Client/BugTracker.Client.csproj", "BugTracker.Client/"]
RUN dotnet restore "BugTracker/BugTracker.csproj"
COPY . .
WORKDIR "/src/BugTracker"
RUN dotnet build "BugTracker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BugTracker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BugTracker.dll"]
