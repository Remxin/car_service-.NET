FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY nupkgs ./nupkgs
COPY nuget.config ./nuget.config
COPY email-service/EmailService/*.csproj ./email-service/EmailService/
RUN dotnet restore email-service/EmailService/EmailService.csproj --configfile ./nuget.config

COPY email-service/EmailService/. ./email-service/EmailService/
RUN dotnet publish email-service/EmailService/EmailService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
RUN apt-get update && apt-get install -y netcat-openbsd

COPY --from=build /app/publish ./




CMD ["dotnet", "WorkshopService.dll"]

