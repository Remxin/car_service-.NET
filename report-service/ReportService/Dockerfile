FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY nupkgs ./nupkgs
COPY nuget.config ./nuget.config
COPY report-service/ReportService/*.csproj ./report-service/ReportService/
RUN dotnet restore report-service/ReportService/ReportService.csproj --configfile ./nuget.config

COPY report-service/ReportService/. ./report-service/ReportService/
RUN dotnet publish report-service/ReportService/ReportService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
RUN apt-get update && apt-get install -y netcat-openbsd

COPY --from=build /app/publish ./




CMD ["dotnet", "AuthService.dll"]

