FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

WORKDIR /app

COPY . ./

RUN dotnet restore "/app/Omicron.Pedidos.Api/Omicron.Pedidos.Api.csproj"
RUN dotnet publish "/app/Omicron.Pedidos.Api/Omicron.Pedidos.Api.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .

ENV TZ America/Mexico_City

ENTRYPOINT ["dotnet", "Omicron.Pedidos.Api.dll"]