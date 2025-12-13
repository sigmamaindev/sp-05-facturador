using Core.Entities;

namespace Core.Interfaces.Services.IKardexService;

public interface IKardexService
{
    Task DecreaseStockForSaleAsync(Invoice invoice);
    Task IncreaseStockForPurchaseAsync(Purchase purchase);
}
