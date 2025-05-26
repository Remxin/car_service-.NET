FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY nupkgs ./nupkgs
COPY nuget.config ./nuget.config
COPY workshop-service/WorkshopService/*.csproj ./workshop-service/WorkshopService/
RUN dotnet restore workshop-service/WorkshopService/WorkshopService.csproj --configfile ./nuget.config

COPY workshop-service/WorkshopService/. ./workshop-service/WorkshopService/
RUN dotnet publish workshop-service/WorkshopService/WorkshopService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
RUN apt-get update && apt-get install -y netcat-openbsd

COPY --from=build /app/publish ./




CMD ["dotnet", "WorkshopService.dll"]

