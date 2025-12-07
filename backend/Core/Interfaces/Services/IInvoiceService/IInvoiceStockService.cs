namespace Core.Interfaces.Services.IInvoiceService;

public interface IInvoiceStockService
{
    Task ReserveStockAsync(int productId, int warehouseId, decimal quantity);
    Task ReturnStockAsync(int productId, int warehouseId, decimal quantity);
}
