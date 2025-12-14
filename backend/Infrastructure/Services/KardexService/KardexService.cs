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

            var ecTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow,
              TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil"));

            var movement = new Kardex
            {
                ProductId = d.ProductId,
                WarehouseId = d.WarehouseId,
                BusinessId = invoice.BusinessId,
                MovementType = MovementType.DECREASE,
                QuantityOut = d.Quantity,
                QuantityIn = 0,
                UnitCost = d.Product!.Price,
                MovementDate = ecTime,
                Reference = $"Factura #{invoice.Id}"
            };

            await context.Kardexes.AddAsync(movement);
        }
    }

    public async Task IncreaseStockForPurchaseAsync(Purchase purchase)
    {
        foreach (var detail in purchase.PurchaseDetails)
        {
            var pw = await context.ProductWarehouses
                .FirstOrDefaultAsync(pw => pw.ProductId == detail.ProductId && pw.WarehouseId == detail.WarehouseId);

            if (pw == null)
            {
                pw = new ProductWarehouse
                {
                    ProductId = detail.ProductId,
                    WarehouseId = detail.WarehouseId,
                    Stock = 0
                };

                await context.ProductWarehouses.AddAsync(pw);
            }

            pw.Stock += detail.Quantity;

            var ecTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow,
              TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil"));

            var movement = new Kardex
            {
                ProductId = detail.ProductId,
                WarehouseId = detail.WarehouseId,
                BusinessId = purchase.BusinessId,
                MovementType = MovementType.INCREASE,
                QuantityIn = detail.Quantity,
                QuantityOut = 0,
                UnitCost = detail.UnitCost,
                TotalCost = detail.Total,
                MovementDate = ecTime,
                Reference = $"Compra #{purchase.Id}"
            };

            await context.Kardexes.AddAsync(movement);
        }
    }
}
