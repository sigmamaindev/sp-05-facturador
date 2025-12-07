using Core.Structure;
using Core.Entities;
using Core.Interfaces.Services.IInvoiceService;

namespace Infrastructure.Services.InvoiceService;

public class InvoiceCalculationService : IInvoiceCalculationService
{
    public InvoiceTotals Calculate(Invoice invoice)
    {
        decimal subtotalBase = 0;
        decimal subtotalWithTaxes = 0;
        decimal discountTotal = 0;
        decimal taxTotal = 0;

        foreach (var detail in invoice.InvoiceDetails)
        {
            var taxableBase = (detail.Quantity * detail.UnitPrice) - detail.Discount;
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

        return new InvoiceTotals(
            subtotalBase,
            subtotalWithTaxes,
            discountTotal,
            taxTotal
        );
    }
}
