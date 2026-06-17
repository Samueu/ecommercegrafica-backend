FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
# Render injeta a porta via env PORT em tempo de execução. Em dev (docker
# compose) o default abaixo se aplica. O ASPNETCORE_URLS é montado no
# ENTRYPOINT para que o $PORT seja expandido em runtime.
ENV PORT=8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ecommercegrafica.csproj", "./"]
COPY ["EcommerceGrafica.Domain/EcommerceGrafica.Domain.csproj", "EcommerceGrafica.Domain/"]
COPY ["EcommerceGrafica.Application/EcommerceGrafica.Application.csproj", "EcommerceGrafica.Application/"]
COPY ["EcommerceGrafica.Repository/EcommerceGrafica.Repository.csproj", "EcommerceGrafica.Repository/"]
COPY ["EcommerceGrafica.Setup/EcommerceGrafica.Setup.csproj", "EcommerceGrafica.Setup/"]
RUN dotnet restore "ecommercegrafica.csproj"
COPY . .
RUN dotnet build "ecommercegrafica.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ecommercegrafica.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN useradd -m appuser
USER appuser

# Shell form para que $PORT seja avaliado em runtime (Render injeta PORT por request de serviço).
ENTRYPOINT ["sh", "-c", "export ASPNETCORE_URLS=http://+:${PORT} && exec dotnet ecommercegrafica.dll"]
