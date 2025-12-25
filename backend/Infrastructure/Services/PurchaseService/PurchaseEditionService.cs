using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Core.Constants;
using Core.Entities;
using Core.DTOs.PurchaseDto;
using Core.Interfaces.Services.IPurchaseService;

namespace Infrastructure.Services.PurchaseService;

public class PurchaseEditionService() : IPurchaseEditionService
{
    public async Task AddPurchaseDetailsAsync(Purchase purchase, IEnumerable<PurchaseDetailCreateReqDto> details)
    {
        foreach (var detail in details)
        {
            var subtotal = detail.Quantity * detail.UnitCost;
            var taxableBase = subtotal - detail.Discount;
            // var taxRate = detail.Tax?.Rate ?? 0;
            // var taxValue = taxableBase * (taxRate / 100);
            // var total = taxableBase + taxValue;

            var netWeight = detail.Quantity;
            var grossWeight = netWeight;

            purchase.PurchaseDetails.Add(new PurchaseDetail
            {                
                NetWeight = netWeight,
                GrossWeight = grossWeight,
                Quantity = detail.Quantity,
                UnitCost = detail.UnitCost,
                TaxId = detail.TaxId,
                Subtotal = taxableBase,
                // TaxRate = taxRate,
                // TaxValue = taxValue,
                // Total = total
            });
        }
    }

    public Purchase BuildPurchase(PurchaseCreateReqDto dto, Supplier supplier, Business business, Establishment establishment, EmissionPoint emissionPoint, User user, DateTime purchaseDate)
    {
        return new Purchase
        {
            Environment = EnvironmentStatus.PROD,
            EmissionTypeCode = EmissionType.NORMAL,
            BusinessName = business.Name,
            Name = business.Name,
            Document = business.Document,
            AccessKey = string.Empty,
            ReceiptType = ReceiptCodeType.INVOICE,
            EstablishmentCode = establishment.Code,
            EmissionPointCode = emissionPoint.Code,
            Sequential = dto.Sequential,
            MainAddress = business.Address,
            IssueDate = purchaseDate,
            EstablishmentAddress = null,
            SpecialTaxpayer = null,
            MandatoryAccounting = null,
            TypeDocumentSubjectDetained = DocumentType.RUC,
            TypeSubjectDetained = string.Empty,
            RelatedParty = "NO",
            BusinessNameSubjectDetained = supplier.BusinessName,
            DocumentSubjectDetained = supplier.Document,
            FiscalPeriod = purchaseDate.ToString("MM/yyyy"),
            SupplierId = supplier.Id,
            Status = PurchaseStatus.DRAFT,
            IsElectronic = false,
            PurchaseDetails = []
        };
    }
}
