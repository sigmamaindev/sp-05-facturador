using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Core.DTOs.PurchaseDto;
using Core.Constants;
using Core.Entities;
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

            var unitMeasureId = detail.UnitMeasureId;

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
                PurchaseId = purchase.Id,
                ProductId = product.Id,
                WarehouseId = warehouse.Id,
                UnitMeasureId = unitMeasure.Id,
                NetWeight = netWeight,
                GrossWeight = grossWeight,
                Quantity = detail.Quantity,
                UnitCost = detail.UnitCost,
                Subtotal = taxableBase,
                Discount = detail.Discount,
                TaxId = product.TaxId,
                TaxRate = taxRate,
                TaxValue = taxValue,
                Total = total
            });
        }
    }

    public Purchase BuildPurchase(PurchaseCreateReqDto dto, User user, Business business, Supplier supplier, DateTime purchaseDate)
    {
        return new Purchase
        {
            BusinessId = business.Id,
            UserId = user.Id,
            Environment = business.SriEnvironment,
            EmissionTypeCode = EmissionType.NORMAL,
            BusinessName = supplier.BusinessName,
            Name = supplier.BusinessName,
            Document = supplier.Document,
            AccessKey = dto.AccessKey,
            ReceiptType = dto.ReceiptType,
            SupportingCode = dto.SupportingCode,
            SupportingDocumentCode = dto.SupportingDocumentCode,
            EstablishmentCode = dto.EstablishmentCode,
            EmissionPointCode = dto.EmissionPointCode,
            Sequential = dto.Sequential,
            MainAddress = dto.MainAddress,
            IssueDate = purchaseDate,
            EstablishmentAddress = dto.EstablishmentAddress,
            SpecialTaxpayer = dto.SpecialTaxpayer,
            MandatoryAccounting = dto.MandatoryAccounting,
            TypeDocumentSubjectDetained = dto.TypeDocumentSubjectDetained,
            TypeSubjectDetained = dto.TypeSubjectDetained,
            RelatedParty = dto.RelatedParty,
            BusinessNameSubjectDetained = dto.BusinessNameSubjectDetained,
            DocumentSubjectDetained = dto.DocumentSubjectDetained,
            AuthorizationDate = purchaseDate,
            FiscalPeriod = purchaseDate.ToString("MM/yyyy"),
            SupplierId = supplier.Id,
            Status = dto.Status,
            IsElectronic = dto.IsElectronic,
            PurchaseDetails = []
        };
    }

    public async Task<Purchase?> CheckPurchaseExistenceAsync(int purchaseId, int businessId)
    {
        return await context.Purchases
            .Include(p => p.Supplier)
            .Include(p => p.PurchaseDetails)
                .ThenInclude(d => d.Product)
                    .ThenInclude(p => p!.Tax)
            .Include(p => p.PurchaseDetails)
                .ThenInclude(d => d.UnitMeasure)
            .Include(p => p.PurchaseDetails)
                .ThenInclude(d => d.Warehouse)
            .Include(p => p.PurchaseDetails)
                .ThenInclude(d => d.Tax)
            .FirstOrDefaultAsync(p => p.Id == purchaseId && p.BusinessId == businessId);
    }

    public async Task UpsertPurchaseDetailsAsync(Purchase purchase, IEnumerable<PurchaseDetailCreateReqDto> details)
    {
        var detailList = details.ToList();

        var incomingKeys = detailList
            .Select(d => (d.ProductId, d.WarehouseId, d.UnitMeasureId))
            .ToHashSet();

        foreach (var oldDetail in purchase.PurchaseDetails.ToList())
        {
            if (!incomingKeys.Contains((oldDetail.ProductId, oldDetail.WarehouseId, oldDetail.UnitMeasureId)))
            {
                context.PurchaseDetails.Remove(oldDetail);
                purchase.PurchaseDetails.Remove(oldDetail);
            }
        }

        foreach (var detail in detailList)
        {
            var product = await context.Products
                .Include(p => p.Tax)
                .FirstOrDefaultAsync(p => p.Id == detail.ProductId)
                ?? throw new InvalidOperationException($"Producto {detail.ProductId} no encontrado");

            var warehouse = await context.Warehouses
                .FirstOrDefaultAsync(w => w.Id == detail.WarehouseId)
                ?? throw new InvalidOperationException($"Bodega {detail.WarehouseId} no encontrada");

            var unitMeasure = await context.UnitMeasures
                .FirstOrDefaultAsync(um => um.Id == detail.UnitMeasureId && um.BusinessId == purchase.BusinessId)
                ?? throw new InvalidOperationException($"Unidad de medida {detail.UnitMeasureId} no encontrada para el negocio actual");

            var taxableBase = (detail.Quantity * detail.UnitCost) - detail.Discount;
            var taxRate = product.Tax?.Rate ?? 0;
            var taxValue = taxableBase * (taxRate / 100);
            var total = taxableBase + taxValue;
            var netWeight = detail.Quantity;
            var grossWeight = netWeight;

            var existingDetail = purchase.PurchaseDetails.FirstOrDefault(d =>
                d.ProductId == detail.ProductId &&
                d.WarehouseId == detail.WarehouseId &&
                d.UnitMeasureId == detail.UnitMeasureId);

            if (existingDetail != null)
            {
                existingDetail.Quantity = detail.Quantity;
                existingDetail.UnitCost = detail.UnitCost;
                existingDetail.Discount = detail.Discount;
                existingDetail.NetWeight = netWeight;
                existingDetail.GrossWeight = grossWeight;
                existingDetail.Subtotal = taxableBase;
                existingDetail.TaxId = product.TaxId;
                existingDetail.TaxRate = taxRate;
                existingDetail.TaxValue = taxValue;
                existingDetail.Total = total;
            }
            else
            {
                purchase.PurchaseDetails.Add(new PurchaseDetail
                {
                    PurchaseId = purchase.Id,
                    ProductId = product.Id,
                    WarehouseId = warehouse.Id,
                    UnitMeasureId = unitMeasure.Id,
                    NetWeight = netWeight,
                    GrossWeight = grossWeight,
                    Quantity = detail.Quantity,
                    UnitCost = detail.UnitCost,
                    Subtotal = taxableBase,
                    Discount = detail.Discount,
                    TaxId = product.TaxId,
                    TaxRate = taxRate,
                    TaxValue = taxValue,
                    Total = total
                });
            }
        }
    }
}
