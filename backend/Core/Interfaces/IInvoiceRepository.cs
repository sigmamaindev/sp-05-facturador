using Core.DTOs;
using Core.DTOs.Invoice;

namespace Core.Interfaces;

public interface IInvoiceRepository
{
    Task<ApiResponse<List<InvoiceSimpleResDto>>> GetInvoicesAsync(string? keyword, int page, int limit);
    Task<ApiResponse<InvoiceComplexResDto>> GetInvoiceByIdAsync(int id);
    Task<ApiResponse<InvoiceSimpleResDto>> CreateInvoiceAsync(InvoiceCreateReqDto invoiceCreateReqDto);
    Task<ApiResponse<InvoiceComplexResDto>> UpdateInvoiceAsync(int invoiceId, InvoiceUpdateReqDto invoiceUpdateReqDto);
}
