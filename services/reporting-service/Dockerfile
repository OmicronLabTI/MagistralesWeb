# FROM mcr.microsoft.com/dotnet/core/sdk:3.1
# RUN apt-get update && apt-get install -y libgdiplus
# WORKDIR /app
# COPY app/. /app/.
# ENTRYPOINT ["dotnet","Omicron.Reporting.Api.dll"]

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app
COPY . ./
RUN dotnet restore "/app/Omicron.Reporting.Api/Omicron.Reporting.Api.csproj"
COPY . ./
RUN dotnet publish "/app/Omicron.Reporting.Api/Omicron.Reporting.Api.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
RUN apt-get update && apt-get install -y libgdiplus
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Omicron.Reporting.Api.dll"]