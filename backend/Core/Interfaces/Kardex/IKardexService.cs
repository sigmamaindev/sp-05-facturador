namespace Core.Interfaces.Kardex;

public interface IKardexService
{
    Task ReserveStockAsync(int productId, int warehouseId, decimal quantity);
    Task DecreaseStockForSaleAsync(int productId, int warehouseId, decimal quantity, int? invoiceId = null);
    Task IncreaseStockForPurchaseAsync(int productId, int warehouseId, decimal quantity, decimal unitCost, int? purchaseId = null);
}
