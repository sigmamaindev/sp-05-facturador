using Core.Entities;
using Core.Structure;

namespace Core.Interfaces.Services.IInvoiceService;

public interface IInvoiceCalculationService
{
    InvoiceTotals Calculate(Invoice invoice);
}
