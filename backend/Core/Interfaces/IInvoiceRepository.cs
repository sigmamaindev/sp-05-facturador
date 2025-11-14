using Core.DTOs;
using Core.DTOs.Invoice;

namespace Core.Interfaces;

public interface IInvoiceRepository
{
    Task<ApiResponse<List<InvoiceResDto>>> GetInvoicesAsync(string? keyword, int page, int limit);
    Task<ApiResponse<InvoiceResDto>> GetInvoiceByIdAsync(int id);
    Task<ApiResponse<InvoiceResDto>> CreateInvoiceAsync(InvoiceCreateReqDto invoiceCreateReqDto);
    Task<ApiResponse<InvoiceResDto>> UpdateInvoiceAsync(int invoiceId, InvoiceUpdateReqDto invoiceUpdateReqDto);
}
