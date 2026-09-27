using FuelLoyalty.Contracts;
using FuelLoyalty.ServiceDefaults.Handlers;
using FuelLoyalty.SharedKernel;
using FuelLoyalty.Sales.Api.Domain.Sales;
using FuelLoyalty.Sales.Api.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FuelLoyalty.Sales.Api.Features.CreateSale
{
    /// <summary>
    /// Satışı kaydeder ve SaleCompleted mesajını Outbox üzerinden yayınlar.
    /// Aynı provizyonla gelen tekrar satışlar yeni kayıt oluşturmaz (idempotency).
    /// </summary>
    public sealed class CreateSaleHandler(
        SalesDbContext db,
        IPublishEndpoint publishEndpoint,
        TimeProvider timeProvider,
        ILogger<CreateSaleHandler> logger) : IHandler
    {
        public async Task<Result<CreateSaleResult>> HandleAsync(CreateSaleRequest request, CancellationToken cancellationToken)
        {
            var existingSaleId = await FindExistingSaleIdAsync(request.AuthorizationId, cancellationToken);
            if (existingSaleId is not null)
            {
                logger.LogWarning("Bu provizyon için satış zaten kayıtlı. Provizyon: {AuthorizationId}", request.AuthorizationId);
                return new CreateSaleResult(existingSaleId.Value, IsDuplicate: true);
            }

            var saleResult = Sale.Create(
                request.AuthorizationId,
                request.StationCode,
                request.CardNumber,
                request.FuelType,
                request.Liters,
                request.Amount,
                timeProvider.GetUtcNow().UtcDateTime);

            if (saleResult.IsFailure)
                return saleResult.Error;

            var sale = saleResult.Value;
            db.Sales.Add(sale);

            // Outbox açık: mesaj RabbitMQ'ya değil, sales içinde OutboxMessage tablosuna yazılır.
            await publishEndpoint.Publish(ToIntegrationEvent(sale), cancellationToken);

            try
            {
                await db.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                // Aynı satış milisaniyeler farkla iki kez geldi ve diğeri önce kaydedildi.
                db.ChangeTracker.Clear();

                var winnerSaleId = await FindExistingSaleIdAsync(request.AuthorizationId, cancellationToken);
                return new CreateSaleResult(winnerSaleId!.Value, IsDuplicate: true);
            }

            logger.LogInformation(
                "Satış kaydedildi. Satış: {SaleId}, Provizyon: {AuthorizationId}, {Liters} L, {Amount} TL",
                sale.Id, sale.AuthorizationId, sale.Liters, sale.Amount);

            return new CreateSaleResult(sale.Id, IsDuplicate: false);
        }

        private Task<Guid?> FindExistingSaleIdAsync(Guid authorizationId, CancellationToken cancellationToken)
            => db.Sales.AsNoTracking()
                .Where(s => s.AuthorizationId == authorizationId)
                .Select(s => (Guid?)s.Id)
                .FirstOrDefaultAsync(cancellationToken);

        private static bool IsUniqueViolation(DbUpdateException ex)
            => ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation };

        private static SaleCompleted ToIntegrationEvent(Sale sale) => new(
            SaleId: sale.Id,
            AuthorizationId: sale.AuthorizationId,
            StationCode: sale.StationCode,
            CardNumber: sale.CardNumber,
            FuelType: sale.FuelType,
            Liters: sale.Liters,
            Amount: sale.Amount,
            SoldAt: sale.SoldAt);
    }
}
