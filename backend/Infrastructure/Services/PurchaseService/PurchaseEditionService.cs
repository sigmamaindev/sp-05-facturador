using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Core.Constants;
using Core.Entities;
using Core.DTOs.PurchaseDto;
using Core.Interfaces.Services.IPurchaseService;

namespace Infrastructure.Services.PurchaseService;

public class PurchaseEditionService(StoreContext context) : IPurchaseEditionService
{
    public async Task AddPurchaseDetailsAsync(Purchase purchase, IEnumerable<PurchaseDetailCreateReqDto> details)
    {
        foreach (var detail in details)
        {
            var product = await context.Products
            .FirstOrDefaultAsync(
                p =>
                p.Id == detail.ProductId &&
                p.BusinessId == purchase.BusinessId &&
                p.IsActive) ??
            throw new Exception($"Producto {detail.ProductId} no encontrado");

            var warehouse = await context.Warehouses
            .FirstOrDefaultAsync(
                w =>
                w.Id == detail.WarehouseId &&
                w.BusinessId == purchase.BusinessId)
            ?? throw new Exception($"Bodega {detail.WarehouseId} no encontrada");

            var subtotal = detail.Quantity * product.Price;
            var taxableBase = subtotal - detail.Discount;
            var taxRate = product.Tax?.Rate ?? 0;
            var taxValue = taxableBase * (taxRate / 100);
            var total = taxableBase + taxValue;

            purchase.PurchaseDetails.Add(new PurchaseDetail
            {
                ProductId = product.Id,
                WarehouseId = warehouse.Id,
                Quantity = detail.Quantity,
                UnitCost = product.Price,
                TaxId = product.TaxId,
                Subtotal = taxableBase,
                TaxRate = taxRate,
                TaxValue = taxValue,
                Total = total
            });
        }
    }

    public Purchase BuildPurchase(PurchaseCreateReqDto dto, Supplier supplier, Business business, Establishment establishment, EmissionPoint emissionPoint, User user, DateTime purchaseDate)
    {
        return new Purchase
        {
            BusinessId = business.Id,
            EstablishmentId = establishment.Id,
            EmissionPointId = emissionPoint.Id,
            SupplierId = supplier.Id,
            PurchaseDate = purchaseDate,
            DocumentNumber = dto.DocumentNumber,
            Reference = dto.Reference,
            Status = PurchaseStatus.DRAFT,
        };
    }
}
