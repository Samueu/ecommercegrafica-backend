FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ecommercegrafica.csproj", "./"]
COPY ["EcommerceGrafica.Application/EcommerceGrafica.Application.csproj", "EcommerceGrafica.Application/"]
COPY ["EcommerceGrafica.Infrastructure/EcommerceGrafica.Infrastructure.csproj", "EcommerceGrafica.Infrastructure/"]
COPY ["EcommerceGrafica.Domain/EcommerceGrafica.Domain.csproj", "EcommerceGrafica.Domain/"]
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

ENTRYPOINT ["dotnet", "ecommercegrafica.dll"]
