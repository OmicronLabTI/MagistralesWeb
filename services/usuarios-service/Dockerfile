FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app
COPY . ./
RUN dotnet restore "/app/Omicron.Usuarios.Api/Omicron.Usuarios.Api.csproj"
COPY . ./
RUN dotnet publish "/app/Omicron.Usuarios.Api/Omicron.Usuarios.Api.csproj" -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Omicron.Usuarios.Api.dll"]