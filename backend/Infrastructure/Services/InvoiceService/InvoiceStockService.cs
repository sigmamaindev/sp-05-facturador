using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Core.Interfaces.Services.IInvoiceService;

namespace Infrastructure.Services.InvoiceService;

public class InvoiceStockService(StoreContext context) : IInvoiceStockService
{
    public async Task ReserveStockAsync(int productId, int warehouseId, decimal quantity)
    {
        var stock = await context.ProductWarehouses
        .FirstOrDefaultAsync(
            pw =>
            pw.ProductId == productId &&
            pw.WarehouseId == warehouseId) ??
        throw new Exception("Stock no encontrado");

        if (stock.Stock < quantity)
        {
            throw new Exception("Stock insuficiente");
        }

        stock.Stock -= quantity;
    }

    public async Task ReturnStockAsync(int productId, int warehouseId, decimal quantity)
    {
        var stock = await context.ProductWarehouses
        .FirstOrDefaultAsync(
            pw =>
            pw.ProductId == productId &&
            pw.WarehouseId == warehouseId);

        if (stock != null)
        {
            stock.Stock += quantity;
        }
    }
}
