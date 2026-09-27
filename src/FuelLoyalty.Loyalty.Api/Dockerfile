# ---------- 1. aþama: derleme ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/FuelLoyalty.Contracts/FuelLoyalty.Contracts.csproj src/FuelLoyalty.Contracts/
COPY src/FuelLoyalty.SharedKernel/FuelLoyalty.SharedKernel.csproj src/FuelLoyalty.SharedKernel/
COPY src/FuelLoyalty.ServiceDefaults/FuelLoyalty.ServiceDefaults.csproj src/FuelLoyalty.ServiceDefaults/
COPY src/FuelLoyalty.Loyalty.Api/FuelLoyalty.Loyalty.Api.csproj src/FuelLoyalty.Loyalty.Api/
RUN dotnet restore src/FuelLoyalty.Loyalty.Api/FuelLoyalty.Loyalty.Api.csproj

COPY src/ src/
RUN dotnet publish src/FuelLoyalty.Loyalty.Api/FuelLoyalty.Loyalty.Api.csproj -c Release -o /app/publish --no-restore

# ---------- 2. aþama: çalýþtýrma ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# Docker health check için curl
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .

USER $APP_UID

EXPOSE 8080
ENTRYPOINT ["dotnet", "FuelLoyalty.Loyalty.Api.dll"]