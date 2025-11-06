FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG NUGET_REPO_URL="https://axity-992382646884.d.codeartifact.us-east-1.amazonaws.com/nuget/axity-nuget-repo/v3/index.json"
ARG NUGET_REPO_NAME="axity/axity-nuget-repo"
ARG NUGET_REPO_USER="aws"
ARG NUGET_AUTH_TOKEN
WORKDIR /source
COPY --link . .

RUN dotnet nuget add source "${NUGET_REPO_URL}" \
    -n "${NUGET_REPO_NAME}" \
    -u "${NUGET_REPO_USER}" \
    -p "${NUGET_AUTH_TOKEN}" \
    --store-password-in-clear-text

RUN dotnet restore Omicron.Invoice.Api/Omicron.Invoice.Api.csproj
COPY --link . .
RUN dotnet publish Omicron.Invoice.Api/Omicron.Invoice.Api.csproj --no-restore -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=es_MX.UTF-8 \
    LANG=es_MX.UTF-8
RUN apk add --no-cache icu-data-full icu-libs tzdata
WORKDIR /app
COPY --link --from=build /app .
USER $APP_UID
EXPOSE 8080

ENTRYPOINT ["dotnet", "Omicron.Invoice.Api.dll"]