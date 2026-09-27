namespace FuelLoyalty.Sales.Api.Features.CreateSale
{
    /// <summary>
    /// Satış kaydının sonucu. IsDuplicate: bu provizyonla satış daha önce kaydedilmişti.
    /// </summary>
    public sealed record CreateSaleResult(Guid SaleId, bool IsDuplicate);
}
