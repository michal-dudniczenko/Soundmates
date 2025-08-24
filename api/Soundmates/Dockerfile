# stage 1
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY *.sln ./
COPY src/Soundmates.Api/*.csproj ./src/Soundmates.Api/
COPY src/Soundmates.Domain/*.csproj ./src/Soundmates.Domain/
COPY src/Soundmates.Infrastructure/*.csproj ./src/Soundmates.Infrastructure/

RUN dotnet restore

# now copy all code
COPY ./src ./src

WORKDIR /src/src/Soundmates.Api
RUN dotnet publish -c Release -o /app/publish

# stage 2
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Soundmates.Api.dll"]
