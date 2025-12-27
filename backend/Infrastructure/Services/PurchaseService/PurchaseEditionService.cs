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
            .Include(p => p.Tax)
            .FirstOrDefaultAsync(p => p.Id == detail.ProductId)
            ?? throw new InvalidOperationException($"Producto {detail.ProductId} no encontrado");

            var warehouse = await context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == detail.WarehouseId)
            ?? throw new InvalidOperationException($"Bodega {detail.WarehouseId} no encontrada");

            var unitMeasureId = detail.UnitMeasureId > 0
                ? detail.UnitMeasureId
                : product.UnitMeasureId;

            var unitMeasure = await context.UnitMeasures
            .FirstOrDefaultAsync(
                um =>
                um.Id == unitMeasureId &&
                um.BusinessId == purchase.BusinessId)
            ?? throw new InvalidOperationException($"Unidad de medida {unitMeasureId} no encontrada para el negocio actual");

            var subtotal = detail.Quantity * detail.UnitCost;
            var taxableBase = subtotal - detail.Discount;
            var taxRate = product.Tax?.Rate ?? 0;
            var taxValue = taxableBase * (taxRate / 100);
            var total = taxableBase + taxValue;

            var netWeight = detail.Quantity;
            var grossWeight = netWeight;

            purchase.PurchaseDetails.Add(new PurchaseDetail
            {
                NetWeight = netWeight,
                GrossWeight = grossWeight,
                Quantity = detail.Quantity,
                UnitCost = detail.UnitCost,
                Subtotal = taxableBase,
                Discount = detail.Discount,
                TaxId = detail.TaxId,
                TaxRate = taxRate,
                TaxValue = taxValue,
                Total = total
            });
        }
    }

    public Purchase BuildPurchase(PurchaseCreateReqDto dto, Business business, Supplier supplier, DateTime purchaseDate)
    {
        return new Purchase
        {
            BusinessId = business.Id,
            Environment = EnvironmentStatus.PROD,
            EmissionTypeCode = EmissionType.NORMAL,
            BusinessName = dto.BusinessName,
            Name = dto.Name,
            Document = dto.Document,
            AccessKey = dto.AccessKey,
            ReceiptType = dto.ReceiptType,
            EstablishmentCode = dto.EstablishmentCode,
            EmissionPointCode = dto.EmissionPointCode,
            Sequential = dto.Sequential,
            MainAddress = dto.MainAddress,
            IssueDate = dto.IssueDate,
            EstablishmentAddress = dto.EstablishmentAddress,
            SpecialTaxpayer = dto.SpecialTaxpayer,
            MandatoryAccounting = dto.MandatoryAccounting,
            TypeDocumentSubjectDetained = dto.TypeDocumentSubjectDetained,
            TypeSubjectDetained = dto.TypeSubjectDetained,
            RelatedParty = dto.RelatedParty,
            BusinessNameSubjectDetained = dto.BusinessNameSubjectDetained,
            DocumentSubjectDetained = dto.DocumentSubjectDetained,
            FiscalPeriod = purchaseDate.ToString("MM/yyyy"),
            SupplierId = supplier.Id,
            Status = dto.Status,
            IsElectronic = dto.IsElectronic,
            PurchaseDetails = []
        };
    }
}
