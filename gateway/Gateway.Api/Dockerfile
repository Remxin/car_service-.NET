FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY nupkgs ./nupkgs
COPY nuget.config ./nuget.config
COPY gateway/Gateway.Api/*.csproj ./gateway/Gateway.Api/
RUN dotnet restore gateway/Gateway.Api/Gateway.Api.csproj --configfile ./nuget.config

COPY gateway/Gateway.Api/. ./gateway/Gateway.Api/
RUN dotnet publish gateway/Gateway.Api/Gateway.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
RUN apt-get update && apt-get install -y netcat-openbsd

COPY --from=build /app/publish ./




CMD ["dotnet", "Gateway.Api.dll"]

