using Core.DTOs;
using Core.DTOs.ReportDto;

namespace Core.Interfaces.Repository;

public interface IReportRepository
{
    Task<ApiResponse<List<SalesReportResDto>>> GetSalesReportAsync(
        string? keyword,
        int? creditDays,
        DateTime? dateFrom,
        DateTime? dateTo,
        int page,
        int limit);

    Task<ApiResponse<List<PurchasesReportResDto>>> GetPurchasesReportAsync(
        string? keyword,
        DateTime? dateFrom,
        DateTime? dateTo,
        int page,
        int limit);
}
