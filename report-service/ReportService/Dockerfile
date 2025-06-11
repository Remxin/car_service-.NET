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

RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    fontconfig \
    libfreetype6 \
    libgdiplus \
    libharfbuzz0b \
    locales \ 
    tzdata \  
    libicu-dev \
    && rm -rf /var/lib/apt/lists/*

RUN echo "pl_PL.UTF-8 UTF-8" > /etc/locale.gen && locale-gen
ENV LANG pl_PL.UTF-8
ENV LC_ALL pl_PL.UTF-8
ENV LANGUAGE pl_PL:en 

RUN apt-get update && apt-get install -y netcat-openbsd

COPY --from=build /app/publish ./
COPY report-service/ReportService/Fonts ./




CMD ["dotnet", "ReportService.dll"]

