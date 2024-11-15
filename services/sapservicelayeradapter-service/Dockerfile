FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build

ARG NUGET_AUTH_TOKEN
ARG NUGET_USER_NAME

WORKDIR /source
COPY --link . .

COPY ./nuget.config.template ./nuget.config
RUN sed -i -e "s/USER/$NUGET_USER_NAME/g" -e "s/PW/$NUGET_AUTH_TOKEN/g" nuget.config 

RUN dotnet restore Omicron.SapServiceLayerAdapter.Api/Omicron.SapServiceLayerAdapter.Api.csproj
COPY --link . .
RUN dotnet publish Omicron.SapServiceLayerAdapter.Api/Omicron.SapServiceLayerAdapter.Api.csproj --no-restore -o /app


FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=es_MX.UTF-8 \
    LANG=es_MX.UTF-8
RUN apk add --no-cache icu-data-full icu-libs tzdata
WORKDIR /app
COPY --link --from=build /app .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Omicron.SapServiceLayerAdapter.Api.dll"]
