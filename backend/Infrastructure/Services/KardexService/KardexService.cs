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
        var reference = $"Factura #{invoice.Sequential}";

        var existingMovements = await context.Kardexes
            .Where(k => k.Reference == reference && k.MovementType == MovementType.SALE)
            .ToListAsync();

        if (existingMovements.Count > 0)
        {
            var restorePairs = existingMovements
                .Select(k => new { k.ProductId, k.WarehouseId, k.QuantityOut })
                .ToList();

            var restoreProductIds = restorePairs.Select(p => p.ProductId).Distinct().ToList();
            var restoreWarehouseIds = restorePairs.Select(p => p.WarehouseId).Distinct().ToList();

            var stocksToRestore = await context.ProductWarehouses
                .Where(pw => restoreProductIds.Contains(pw.ProductId) && restoreWarehouseIds.Contains(pw.WarehouseId))
                .ToListAsync();

            var restoreStockByKey = stocksToRestore.ToDictionary(pw => (pw.ProductId, pw.WarehouseId));

            foreach (var movement in restorePairs)
            {
                if (restoreStockByKey.TryGetValue((movement.ProductId, movement.WarehouseId), out var stock))
                {
                    stock.Stock += movement.QuantityOut;
                }
            }

            context.Kardexes.RemoveRange(existingMovements);
        }

        var details = invoice.InvoiceDetails.Count > 0
            ? invoice.InvoiceDetails.ToList()
            : await context.InvoiceDetails
                .Include(d => d.Product)
                .Include(d => d.UnitMeasure)
                .Where(d => d.InvoiceId == invoice.Id)
                .ToListAsync();

        if (details.Count == 0)
        {
            return;
        }

        var productInfoById = await LoadProductInfoAsync(details);
        var unitMeasureFactorById = await LoadUnitMeasureFactorAsync(details.Select(d => d.UnitMeasureId));

        var unknownProductId = details
            .Select(d => d.ProductId)
            .Distinct()
            .FirstOrDefault(id => !productInfoById.ContainsKey(id));

        if (unknownProductId != 0)
        {
            throw new InvalidOperationException($"Producto {unknownProductId} no encontrado");
        }

        var stockLines = details
            .Where(d => productInfoById[d.ProductId].Type != ProductTypes.SERVICE)
            .ToList();

        if (stockLines.Count == 0)
        {
            return;
        }

        var requiredPairs = stockLines
            .Select(d => new { d.ProductId, WarehouseId = d.WarehouseId })
            .ToList();

        if (requiredPairs.Any(p => p.WarehouseId is null))
        {
            var first = stockLines.First(d => d.WarehouseId is null);
            throw new InvalidOperationException($"La bodega es obligatoria para el producto {first.Product?.Name ?? first.ProductId.ToString()}");
        }

        var productIds = requiredPairs.Select(p => p.ProductId).Distinct().ToList();
        var warehouseIds = requiredPairs.Select(p => p.WarehouseId!.Value).Distinct().ToList();

        var stocks = await context.ProductWarehouses
            .Where(pw => productIds.Contains(pw.ProductId) && warehouseIds.Contains(pw.WarehouseId))
            .ToListAsync();

        var stockByKey = stocks.ToDictionary(pw => (pw.ProductId, pw.WarehouseId));

        var movements = new List<Kardex>(stockLines.Count);
        var movementDate = GetEcuadorTimeNow();

        foreach (var detail in stockLines)
        {
            var productName = productInfoById.TryGetValue(detail.ProductId, out var p)
                ? p.Name
                : detail.ProductId.ToString();

            if (detail.Quantity <= 0)
            {
                throw new InvalidOperationException($"Cantidad inválida para el producto {productName}");
            }

            var warehouseId = detail.WarehouseId!.Value;

            if (!stockByKey.TryGetValue((detail.ProductId, warehouseId), out var stock))
            {
                throw new InvalidOperationException($"El producto {productName} no tiene stock configurado en la bodega seleccionada");
            }

            if (detail.UnitMeasureId <= 0)
            {
                throw new InvalidOperationException($"Unidad de medida inválida para el producto {productName}");
            }

            var factorBase = detail.UnitMeasure?.FactorBase
                ?? unitMeasureFactorById.GetValueOrDefault(detail.UnitMeasureId, 0m);
            if (factorBase <= 0)
            {
                throw new InvalidOperationException($"Factor de unidad inválido para el producto {productName}");
            }

            var quantityBase = Math.Round(detail.Quantity * factorBase, 4);

            if (stock.Stock < quantityBase)
            {
                throw new InvalidOperationException($"Stock insuficiente para el producto {productName}.");
            }

            stock.Stock -= quantityBase;

            var unitCostBase = Math.Round(detail.UnitPrice / factorBase, 4);

            movements.Add(new Kardex
            {
                ProductId = detail.ProductId,
                WarehouseId = warehouseId,
                BusinessId = invoice.BusinessId,
                MovementType = MovementType.SALE,
                QuantityOut = quantityBase,
                QuantityIn = 0,
                UnitCost = unitCostBase,
                TotalCost = Math.Round(quantityBase * unitCostBase, 4),
                MovementDate = movementDate,
                Reference = $"Factura #{invoice.Sequential}"
            });
        }

        await context.Kardexes.AddRangeAsync(movements);
    }

    public async Task IncreaseStockForPurchaseAsync(Purchase purchase)
    {
        var details = purchase.PurchaseDetails.Count > 0
            ? purchase.PurchaseDetails.ToList()
            : await context.PurchaseDetails
                .Include(d => d.Product)
                .Include(d => d.UnitMeasure)
                .Where(d => d.PurchaseId == purchase.Id)
                .ToListAsync();

        if (details.Count == 0)
        {
            return;
        }

        var productInfoById = await LoadProductInfoAsync(details);
        var unitMeasureFactorById = await LoadUnitMeasureFactorAsync(details.Select(d => d.UnitMeasureId));

        var unknownProductId = details
            .Select(d => d.ProductId)
            .Distinct()
            .FirstOrDefault(id => !productInfoById.ContainsKey(id));

        if (unknownProductId != 0)
        {
            throw new InvalidOperationException($"Producto {unknownProductId} no encontrado");
        }

        var stockLines = details
            .Where(d => productInfoById[d.ProductId].Type != ProductTypes.SERVICE)
            .ToList();

        if (stockLines.Count == 0)
        {
            return;
        }

        var productIds = stockLines.Select(d => d.ProductId).Distinct().ToList();
        var warehouseIds = stockLines.Select(d => d.WarehouseId).Distinct().ToList();

        var stocks = await context.ProductWarehouses
            .Where(pw => productIds.Contains(pw.ProductId) && warehouseIds.Contains(pw.WarehouseId))
            .ToListAsync();

        var stockByKey = stocks.ToDictionary(pw => (pw.ProductId, pw.WarehouseId));

        var movements = new List<Kardex>(stockLines.Count);
        var movementDate = GetEcuadorTimeNow();

        foreach (var detail in stockLines)
        {
            var productName = productInfoById.TryGetValue(detail.ProductId, out var p)
                ? p.Name
                : detail.ProductId.ToString();

            if (detail.Quantity <= 0)
            {
                throw new InvalidOperationException($"Cantidad inválida para el producto {productName}");
            }

            if (!stockByKey.TryGetValue((detail.ProductId, detail.WarehouseId), out var stock))
            {
                stock = new ProductWarehouse
                {
                    ProductId = detail.ProductId,
                    WarehouseId = detail.WarehouseId,
                    Stock = 0
                };

                context.ProductWarehouses.Add(stock);
                stockByKey[(detail.ProductId, detail.WarehouseId)] = stock;
            }

            if (detail.UnitMeasureId <= 0)
            {
                throw new InvalidOperationException($"Unidad de medida inválida para el producto {productName}");
            }

            var factorBase = detail.UnitMeasure?.FactorBase
                ?? unitMeasureFactorById.GetValueOrDefault(detail.UnitMeasureId, 0m);
            if (factorBase <= 0)
            {
                throw new InvalidOperationException($"Factor de unidad inválido para el producto {productName}");
            }

            var quantityBase = Math.Round(detail.Quantity * factorBase, 4);
            stock.Stock += quantityBase;

            var unitCostBase = Math.Round(detail.UnitCost / factorBase, 4);

            movements.Add(new Kardex
            {
                ProductId = detail.ProductId,
                WarehouseId = detail.WarehouseId,
                BusinessId = purchase.BusinessId,
                MovementType = MovementType.PURCHASE,
                QuantityIn = quantityBase,
                QuantityOut = 0,
                UnitCost = unitCostBase,
                TotalCost = Math.Round(quantityBase * unitCostBase, 4),
                MovementDate = movementDate,
                Reference = $"Compra #{purchase.Sequential}"
            });
        }

        await context.Kardexes.AddRangeAsync(movements);
    }

    private async Task<Dictionary<int, (string Name, string Type)>> LoadProductInfoAsync(IEnumerable<InvoiceDetail> details)
    {
        var info = new Dictionary<int, (string Name, string Type)>();

        foreach (var d in details)
        {
            if (d.Product != null && !info.ContainsKey(d.ProductId))
            {
                info[d.ProductId] = (d.Product.Name, d.Product.Type);
            }
        }

        var missingIds = details
            .Where(d => d.Product == null && !info.ContainsKey(d.ProductId))
            .Select(d => d.ProductId)
            .Distinct()
            .ToList();

        if (missingIds.Count == 0)
        {
            return info;
        }

        var missing = await context.Products
            .Where(p => missingIds.Contains(p.Id))
            .Select(p => new { p.Id, p.Name, p.Type })
            .ToListAsync();

        foreach (var p in missing)
        {
            info[p.Id] = (p.Name, p.Type);
        }

        return info;
    }

    private async Task<Dictionary<int, (string Name, string Type)>> LoadProductInfoAsync(IEnumerable<PurchaseDetail> details)
    {
        var info = new Dictionary<int, (string Name, string Type)>();

        foreach (var d in details)
        {
            if (d.Product != null && !info.ContainsKey(d.ProductId))
            {
                info[d.ProductId] = (d.Product.Name, d.Product.Type);
            }
        }

        var missingIds = details
            .Where(d => d.Product == null && !info.ContainsKey(d.ProductId))
            .Select(d => d.ProductId)
            .Distinct()
            .ToList();

        if (missingIds.Count == 0)
        {
            return info;
        }

        var missing = await context.Products
            .Where(p => missingIds.Contains(p.Id))
            .Select(p => new { p.Id, p.Name, p.Type })
            .ToListAsync();

        foreach (var p in missing)
        {
            info[p.Id] = (p.Name, p.Type);
        }

        return info;
    }

    private async Task<Dictionary<int, decimal>> LoadUnitMeasureFactorAsync(IEnumerable<int> unitMeasureIds)
    {
        var ids = unitMeasureIds.Where(id => id > 0).Distinct().ToList();
        if (ids.Count == 0)
        {
            return [];
        }

        return await context.UnitMeasures
            .Where(um => ids.Contains(um.Id))
            .ToDictionaryAsync(um => um.Id, um => um.FactorBase);
    }

    private static DateTime GetEcuadorTimeNow()
    {
        try
        {
            return TimeZoneInfo.ConvertTime(
                DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil"));
        }
        catch
        {
            return DateTime.UtcNow;
        }
    }
}
