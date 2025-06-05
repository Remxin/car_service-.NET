FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY nupkgs ./nupkgs
COPY nuget.config ./nuget.config
COPY file-upload-service/FileUploadService/*.csproj ./file-upload-service/FileUploadService/
RUN dotnet restore file-upload-service/FileUploadService/FileUploadService.csproj --configfile ./nuget.config

COPY file-upload-service/FileUploadService/. ./file-upload-service/FileUploadService/
RUN dotnet publish file-upload-service/FileUploadService/FileUploadService.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
RUN apt-get update && apt-get install -y netcat-openbsd

COPY --from=build /app/publish ./




CMD ["dotnet", "FileUploadService.dll"]

