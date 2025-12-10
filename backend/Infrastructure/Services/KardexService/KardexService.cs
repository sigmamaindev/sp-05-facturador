using System;
using Core.Entities;
using Core.Interfaces.Kardex;
using Core.Interfaces.Specifications.ProductWarehouseSpecification;
using Infrastructure.Data;
using Infrastructure.Specification.ProductWarehouseSpecification;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.KardexService;

public class KardexService(StoreContext context) : IKardexService
{
    public async Task ReserveStockAsync(int productId, int warehouseId, decimal quantity)
    {
        var stock = await GetProductWarehouseAsync(productId, warehouseId);

        if (stock.Stock < quantity)
        {
            throw new Exception("Stock insuficiente para reservar");
        }

        stock.Stock -= quantity;
        await context.SaveChangesAsync();
    }

    public async Task DecreaseStockForSaleAsync(int productId, int warehouseId, decimal quantity, int? invoiceId = null)
    {
        var stock = await GetProductWarehouseAsync(productId, warehouseId);

        if (stock.Stock < quantity)
        {
            throw new Exception("Stock insuficiente para la venta");
        }

        stock.Stock -= quantity;

        await LogMovementAsync(
            productId,
            warehouseId,
            quantityIn: 0,
            quantityOut: quantity,
            unitCost: stock.Product?.Price ?? 0,
            movementType: "SALIDA",
            invoiceId: invoiceId);
    }

    public async Task IncreaseStockForPurchaseAsync(int productId, int warehouseId, decimal quantity, decimal unitCost, int? purchaseId = null)
    {
        var stock = await GetProductWarehouseAsync(productId, warehouseId);

        stock.Stock += quantity;

        await LogMovementAsync(
            productId,
            warehouseId,
            quantityIn: quantity,
            quantityOut: 0,
            unitCost: unitCost,
            movementType: "ENTRADA",
            purchaseId: purchaseId);
    }

    private async Task<ProductWarehouse> GetProductWarehouseAsync(int productId, int warehouseId)
    {
        IProductWarehouseByProductAndWarehouseSpecification specification = new ProductWarehouseByProductAndWarehouseSpecification(productId, warehouseId);
        var stock = await specification.Apply(context.ProductWarehouses).FirstOrDefaultAsync();

        return stock ?? throw new Exception("Registro de inventario no encontrado");
    }

    private async Task LogMovementAsync(
        int productId,
        int warehouseId,
        decimal quantityIn,
        decimal quantityOut,
        decimal unitCost,
        string movementType,
        int? purchaseId = null,
        int? invoiceId = null)
    {
        var kardexEntry = new Kardex
        {
            ProductId = productId,
            WarehouseId = warehouseId,
            MovementDate = DateTime.UtcNow,
            QuantityIn = quantityIn,
            QuantityOut = quantityOut,
            UnitCost = unitCost,
            TotalCost = unitCost * (quantityIn > 0 ? quantityIn : quantityOut),
            MovementType = movementType,
            PurchaseId = purchaseId,
            InvoiceId = invoiceId
        };

        context.Kardexes.Add(kardexEntry);
        await context.SaveChangesAsync();
    }
}
