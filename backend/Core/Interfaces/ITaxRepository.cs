using Core.DTOs;
using Core.DTOs.Tax;

namespace Core.Interfaces;

public interface ITaxRepository
{
    Task<ApiResponse<List<TaxResDto>>> GetTaxesAsync(string? keyword, int page, int limit);
}
