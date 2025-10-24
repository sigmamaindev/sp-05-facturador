using Core.DTOs;
using Core.DTOs.Invoice;

namespace Core.Interfaces;

public interface IInvoiceRepository
{
    Task<ApiResponse<InvoiceResDto>> CreateDraftInvoiceAsync(InvoiceCreateReqDto invoiceCreateReqDto);
}
