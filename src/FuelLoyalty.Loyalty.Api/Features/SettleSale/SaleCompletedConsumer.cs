using FuelLoyalty.Contracts;
using MassTransit;

namespace FuelLoyalty.Loyalty.Api.Features.SettleSale
{
    /// <summary>
    /// RabbitMQ'dan gelen SaleCompleted mesajını handler'a iletir.
    /// İş mantığı içermez: mesaj dünyası ile uygulama arasındaki köprüdür.
    /// </summary>
    public sealed class SaleCompletedConsumer(
        SettleSaleHandler handler,
        ILogger<SaleCompletedConsumer> logger) : IConsumer<SaleCompleted>
    {
        public async Task Consume(ConsumeContext<SaleCompleted> context)
        {
            var result = await handler.HandleAsync(context.Message, context.CancellationToken);

            // İş hataları (provizyon yok, zaten tamamlanmış) tekrar denemeyle düzelmez: logla ve geç.
            if (result.IsFailure)
            {
                logger.LogWarning(
                    "Satış işlenmedi. Satış: {SaleId}, Kod: {Code}, Neden: {Reason}",
                    context.Message.SaleId, result.Error.Code, result.Error.Message);
            }
        }
    }
}
