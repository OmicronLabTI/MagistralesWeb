FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app
COPY . ./
RUN dotnet restore "/app/Omicron.SapAdapter.Api/Omicron.SapAdapter.Api.csproj"
COPY . ./
RUN dotnet publish "/app/Omicron.SapAdapter.Api/Omicron.SapAdapter.Api.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Omicron.SapAdapter.Api.dll"]