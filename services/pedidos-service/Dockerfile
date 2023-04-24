FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

WORKDIR /app

COPY . ./

RUN dotnet restore "/app/Omicron.Pedidos.Api/Omicron.Pedidos.Api.csproj"
RUN dotnet publish "/app/Omicron.Pedidos.Api/Omicron.Pedidos.Api.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0.12-alpine3.17
RUN apt-get update && apt-get install -y libgdiplus
RUN apt-get install -y libfreetype6
RUN apt-get install -y libfontconfig1
WORKDIR /app
COPY --from=build-env /app/out .

RUN apk add icu-libs --no-cache tzdata
RUN apk update

ENV TZ America/Mexico_City
EXPOSE 5103
ENTRYPOINT ["dotnet", "Omicron.Pedidos.Api.dll"]