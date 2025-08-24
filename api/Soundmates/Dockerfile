# stage 1
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY *.sln .
COPY ./src ./src

RUN dotnet restore

WORKDIR /src/src/Soundmates.Api

RUN dotnet publish -c Release -o /app/publish

# stage 2
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Soundmates.Api.dll"]