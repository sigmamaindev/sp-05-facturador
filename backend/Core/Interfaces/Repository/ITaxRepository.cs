using Core.DTOs;
using Core.DTOs.TaxDto;

namespace Core.Interfaces.Repository;

public interface ITaxRepository
{
    Task<ApiResponse<List<TaxResDto>>> GetTaxesAsync(string? keyword, int page, int limit);
}
