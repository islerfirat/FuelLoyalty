# ---------- 1. aþama: derleme ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/FuelLoyalty.Contracts/FuelLoyalty.Contracts.csproj src/FuelLoyalty.Contracts/
COPY src/FuelLoyalty.SharedKernel/FuelLoyalty.SharedKernel.csproj src/FuelLoyalty.SharedKernel/
COPY src/FuelLoyalty.Portal/FuelLoyalty.Portal.csproj src/FuelLoyalty.Portal/
RUN dotnet restore src/FuelLoyalty.Portal/FuelLoyalty.Portal.csproj

COPY src/ src/
RUN dotnet publish src/FuelLoyalty.Portal/FuelLoyalty.Portal.csproj -c Release -o /app/publish --no-restore

# ---------- 2. aþama: çalýþtýrma ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Data Protection anahtar klasörü: uygulama kullanýcýsý yazabilsin
RUN mkdir -p /app/keys && chown $APP_UID /app/keys

COPY --from=build /app/publish .

USER $APP_UID

EXPOSE 8080
ENTRYPOINT ["dotnet", "FuelLoyalty.Portal.dll"]