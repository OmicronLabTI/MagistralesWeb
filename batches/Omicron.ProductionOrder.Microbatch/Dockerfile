FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build-env

WORKDIR /app
COPY . ./

RUN dotnet restore "/app/Omicron.ProductionOrder.Batch/Omicron.ProductionOrder.Batch.csproj"
RUN dotnet publish "/app/Omicron.ProductionOrder.Batch/Omicron.ProductionOrder.Batch.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine
WORKDIR /app
COPY --from=build-env /app/out .

RUN apk add icu-libs --no-cache tzdata
RUN apk update

ENV TZ=America/Mexico_City

ENTRYPOINT ["dotnet", "Omicron.ProductionOrder.Batch.dll"]
