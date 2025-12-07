using Core.Entities;
using Core.DTOs.InvoiceDto;

namespace Core.Interfaces.Services.IInvoiceService;

public interface IInvoiceDtoFactory
{
    InvoiceSimpleResDto InvoiceSimpleRes(Invoice invoice);
    InvoiceComplexResDto InvoiceComplexRes(Invoice invoice);
}
