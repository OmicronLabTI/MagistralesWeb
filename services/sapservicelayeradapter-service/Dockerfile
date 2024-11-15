FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build-env
WORKDIR /app
COPY --link . ./
RUN dotnet restore "/app/Omicron.SapServiceLayerAdapter.Api/Omicron.SapServiceLayerAdapter.Api.csproj"
RUN dotnet publish "/app/Omicron.SapServiceLayerAdapter.Api/Omicron.SapServiceLayerAdapter.Api.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=es_MX.UTF-8 \
    LANG=es_MX.UTF-8
RUN apk add --no-cache icu-data-full icu-libs tzdata
WORKDIR /app
COPY --link --from=build-env /app/out .
USER $APP_UID
EXPOSE 8080

ENTRYPOINT ["dotnet", "Omicron.SapServiceLayerAdapter.Api.dll"]