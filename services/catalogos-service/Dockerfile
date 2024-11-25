FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /source
COPY --link . .

RUN dotnet restore Omicron.Catalogos.Api/Omicron.Catalogos.Api.csproj
COPY --link . .
RUN dotnet publish Omicron.Catalogos.Api/Omicron.Catalogos.Api.csproj --no-restore -o /app


FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=es_MX.UTF-8 \
    LANG=es_MX.UTF-8
RUN apk add --no-cache icu-data-full icu-libs tzdata
WORKDIR /app
COPY --link --from=build /app .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Omicron.Catalogos.Api.dll"]