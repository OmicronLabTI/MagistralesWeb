FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build-env
WORKDIR /app
COPY --link . ./
RUN dotnet restore "/app/Omicron.SapAdapter.Api/Omicron.SapAdapter.Api.csproj"
RUN dotnet add "Omicron.SapAdapter.DependencyInjection/Omicron.SapAdapter.DependencyInjection.csproj" package Microsoft.Identity.Client --version 4.64.1
RUN dotnet add "Omicron.SapAdapter.Repository/Omicron.SapAdapter.Entities.csproj" package Microsoft.Identity.Client --version 4.64.1
RUN dotnet publish "/app/Omicron.SapAdapter.Api/Omicron.SapAdapter.Api.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=es_MX.UTF-8 \
    LANG=es_MX.UTF-8
RUN apk add --no-cache icu-data-full icu-libs tzdata
WORKDIR /app
COPY --link --from=build-env /app/out .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "Omicron.SapAdapter.Api.dll"]