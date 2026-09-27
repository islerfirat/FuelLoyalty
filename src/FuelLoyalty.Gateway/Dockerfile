# ---------- 1. aþama: derleme ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/FuelLoyalty.Gateway/FuelLoyalty.Gateway.csproj src/FuelLoyalty.Gateway/
RUN dotnet restore src/FuelLoyalty.Gateway/FuelLoyalty.Gateway.csproj

COPY src/FuelLoyalty.Gateway/ src/FuelLoyalty.Gateway/
RUN dotnet publish src/FuelLoyalty.Gateway/FuelLoyalty.Gateway.csproj -c Release -o /app/publish --no-restore

# ---------- 2. aþama: çalýþtýrma ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

USER $APP_UID

EXPOSE 8080
ENTRYPOINT ["dotnet", "FuelLoyalty.Gateway.dll"]