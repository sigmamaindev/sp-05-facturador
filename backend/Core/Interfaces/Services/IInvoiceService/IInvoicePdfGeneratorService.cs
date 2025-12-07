using Core.DTOs.InvoiceDto;

namespace Core.Interfaces.Services.IInvoiceService;

public interface IInvoicePdfGeneratorService
{
    byte[] Generate(InvoiceComplexResDto invoice);
}
