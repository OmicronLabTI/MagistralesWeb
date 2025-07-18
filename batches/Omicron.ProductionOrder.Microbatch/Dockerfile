FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

WORKDIR /app
COPY . ./

RUN dotnet restore "/app/Omicron.ProductionOrder.Batch/Omicron.ProductionOrder.Batch.csproj"
RUN dotnet publish "/app/Omicron.ProductionOrder.Batch/Omicron.ProductionOrder.Batch.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine3.20
WORKDIR /app
COPY --from=build-env /app/out .

RUN apk add icu-libs --no-cache tzdata
RUN apk update

ENV TZ=America/Mexico_City

ENTRYPOINT ["dotnet", "Omicron.ProductionOrder.Batch.dll"]