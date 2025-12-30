using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Core.Interfaces.Services.IInvoiceService;

namespace Infrastructure.Services.InvoiceService;

public class InvoiceStockService(StoreContext context) : IInvoiceStockService
{
    public async Task ReserveStockAsync(int productId, int warehouseId, int unitMeasureId, decimal quantity)
    {
        if (quantity <= 0)
        {
            throw new InvalidOperationException("Cantidad inválida");
        }

        var factorBase = await GetFactorBaseAsync(unitMeasureId);
        var quantityBase = quantity * factorBase;

        var stock = await context.ProductWarehouses
        .FirstOrDefaultAsync(
            pw =>
            pw.ProductId == productId &&
            pw.WarehouseId == warehouseId) ??
        throw new Exception("Stock no encontrado");

        if (stock.Stock < quantityBase)
        {
            throw new Exception("Stock insuficiente");
        }

        stock.Stock -= quantityBase;
    }

    public async Task ReturnStockAsync(int productId, int warehouseId, int unitMeasureId, decimal quantity)
    {
        if (quantity <= 0)
        {
            return;
        }

        var factorBase = await GetFactorBaseAsync(unitMeasureId);
        var quantityBase = quantity * factorBase;

        var stock = await context.ProductWarehouses
        .FirstOrDefaultAsync(
            pw =>
            pw.ProductId == productId &&
            pw.WarehouseId == warehouseId);

        if (stock != null)
        {
            stock.Stock += quantityBase;
        }
    }

    private async Task<decimal> GetFactorBaseAsync(int unitMeasureId)
    {
        var factorBase = await context.UnitMeasures
            .Where(um => um.Id == unitMeasureId)
            .Select(um => um.FactorBase)
            .FirstOrDefaultAsync();

        if (factorBase <= 0)
        {
            throw new InvalidOperationException("Factor de unidad inválido");
        }

        return factorBase;
    }
}
