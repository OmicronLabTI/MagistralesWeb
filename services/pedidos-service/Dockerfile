FROM mcr.microsoft.com/dotnet/core/sdk:3.1
RUN apt-get update && apt-get install -y libgdiplus
WORKDIR /app
COPY app/. /app/.
ENTRYPOINT ["dotnet","Omicron.Pedidos.Api.dll"]