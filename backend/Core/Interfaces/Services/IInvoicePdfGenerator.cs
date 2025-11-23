using Core.DTOs.Invoice;

namespace Core.Interfaces.Services;

public interface IInvoicePdfGenerator
{
    byte[] Generate(InvoiceComplexResDto invoice);
}
