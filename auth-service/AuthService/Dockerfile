FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY nupkgs ./nupkgs
COPY nuget.config ./nuget.config
COPY auth-service/AuthService/*.csproj ./auth-service/AuthService/
RUN dotnet restore auth-service/AuthService/AuthService.csproj --configfile ./nuget.config

COPY auth-service/AuthService/. ./auth-service/AuthService/
RUN dotnet publish auth-service/AuthService/AuthService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
RUN apt-get update && apt-get install -y netcat-openbsd

COPY --from=build /app/publish ./




CMD ["dotnet", "AuthService.dll"]

