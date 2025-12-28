using Core.DTOs;
using Core.DTOs.ARDto;

namespace Core.Interfaces.Repository;

public interface IAccountsReceivableRepository
{
    Task<ApiResponse<List<ARSimpleResDto>>> GetAccountsReceivablesAsync(string? keyword, int page, int limit);
    Task<ApiResponse<ARComplexResDto>> GetAccountsReceivableByIdAsync(int id);
}

