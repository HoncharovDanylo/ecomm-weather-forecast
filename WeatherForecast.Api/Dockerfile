FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy csproj files first for layer caching
COPY ["WeatherForecast.Api/WeatherForecast.Api.csproj", "WeatherForecast.Api/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Repositories/Repositories.csproj", "Repositories/"]
COPY ["Services/Services.csproj", "Services/"]
COPY ["DomainEntities/DomainEntities.csproj", "DomainEntities/"]
COPY ["Scheduling/Scheduling.csproj", "Scheduling/"]
COPY ["UnitTests/Tests.csproj", "Tests/"]

RUN dotnet restore "WeatherForecast.Api/WeatherForecast.Api.csproj"

COPY . .
WORKDIR /src/WeatherForecast.Api
RUN dotnet publish -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WeatherForecast.Api.dll"]
