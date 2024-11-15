FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /source
COPY --link . .

RUN dotnet restore Omicron.Warehouses.Api/Omicron.Warehouses.Api.csproj

RUN dotnet add Omicron.Warehouses.Api/Omicron.Warehouses.Api.csproj package Azure.Identity --version 1.12.1

COPY --link . .
RUN dotnet publish Omicron.Warehouses.Api/Omicron.Warehouses.Api.csproj --no-restore -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=es_MX.UTF-8 \
    LANG=es_MX.UTF-8
RUN apk add --no-cache icu-data-full icu-libs tzdata
WORKDIR /app
COPY --link --from=build /app .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Omicron.Warehouses.Api.dll"]