using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Core.Constants;
using Core.Entities;
using Core.Interfaces.Services.IKardexService;

namespace Infrastructure.Services.KardexService;

public class KardexService(StoreContext context) : IKardexService
{
    public async Task DecreaseStockForSaleAsync(Invoice invoice)
    {
        foreach (var d in invoice.InvoiceDetails)
        {
            var pw = await context.ProductWarehouses
                .FirstOrDefaultAsync(
                    x =>
                    x.ProductId == d.ProductId &&
                    x.WarehouseId == d.WarehouseId) ??
            throw new Exception($"El producto {d.Product!.Name} no tiene stock configurado en la bodega seleccionada");


            if (pw.Stock < d.Quantity)
            {
                throw new Exception($"Stock insuficiente para el producto {d.Product!.Name}.");
            }

            var movement = new Kardex
            {
                ProductId = d.ProductId,
                WarehouseId = d.WarehouseId,
                BusinessId = invoice.BusinessId,
                MovementType = MovementType.DECREASE,
                QuantityOut = d.Quantity,
                QuantityIn = 0,
                UnitCost = d.Product!.Price,
                MovementDate = DateTime.UtcNow,
                Reference = $"Factura #{invoice.Id}"
            };

            await context.Kardexes.AddAsync(movement);
        }
    }

    public Task IncreaseStockForPurchaseAsync(Purchase purchase)
    {
        throw new NotImplementedException();
    }
}
