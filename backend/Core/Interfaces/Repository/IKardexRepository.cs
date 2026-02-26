using Core.DTOs;
using Core.DTOs.KardexDto;

namespace Core.Interfaces.Repository;

public interface IKardexRepository
{
    Task<ApiResponse<List<KardexResDto>>> GetKardexAsync(string? keyword, int page, int limit);
    Task<ApiResponse<KardexReportWrapperDto>> GetKardexReportAsync(int productId, DateTime dateFrom, DateTime dateTo);
}
