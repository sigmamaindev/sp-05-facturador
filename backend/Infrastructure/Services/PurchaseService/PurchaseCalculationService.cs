using Core.Structure;
using Core.Entities;
using Core.Interfaces.Services.IPurchaseService;

namespace Infrastructure.Services.PurchaseService;

public class PurchaseCalculationService : IPurchaseCalculationService
{
    public PurchaseTotals Calculate(Purchase purchase)
    {
        decimal subtotalBase = 0;
        decimal subtotalWithTaxes = 0;
        decimal discountTotal = 0;
        decimal taxTotal = 0;

        foreach (var detail in purchase.PurchaseDetails)
        {
            var taxableBase = (detail.Quantity * detail.UnitCost) - detail.Discount;
            var taxRate = detail.Tax?.Rate ?? detail.TaxRate;
            var taxValue = taxableBase * (taxRate / 100);
            var total = taxableBase + taxValue;

            detail.Subtotal = taxableBase;
            detail.TaxRate = taxRate;
            detail.TaxValue = taxValue;
            detail.Total = total;

            subtotalBase += taxableBase;
            discountTotal += detail.Discount;
            taxTotal += taxValue;
            subtotalWithTaxes += total;
        }

        return new PurchaseTotals(
            subtotalBase,
            subtotalWithTaxes,
            discountTotal,
            taxTotal
        );
    }
}
