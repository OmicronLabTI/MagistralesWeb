FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build-env
ARG NUGET_REPO_URL="https://axity-992382646884.d.codeartifact.us-east-1.amazonaws.com/nuget/axity-nuget-repo/v3/index.json"
ARG NUGET_REPO_NAME="axity/axity-nuget-repo"
ARG NUGET_REPO_USER="aws"
ARG NUGET_AUTH_TOKEN
WORKDIR /app
COPY . ./

RUN dotnet nuget add source "${NUGET_REPO_URL}" -n "${NUGET_REPO_NAME}" -u "${NUGET_REPO_USER}" -p "${NUGET_AUTH_TOKEN}" --store-password-in-clear-text
RUN dotnet restore "/app/Omicron.ProductionOrder.Batch/Omicron.ProductionOrder.Batch.csproj"
RUN dotnet publish "/app/Omicron.ProductionOrder.Batch/Omicron.ProductionOrder.Batch.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app
COPY --from=build-env /app/out .

RUN apk add icu-libs --no-cache tzdata
RUN apk update

ENV TZ=America/Mexico_City

ENTRYPOINT ["dotnet", "Omicron.ProductionOrder.Batch.dll"]