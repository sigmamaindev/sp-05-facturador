using Core.DTOs;
using Core.DTOs.APDto;

namespace Core.Interfaces.Repository;

public interface IAccountsPayableRepository
{
    Task<ApiResponse<List<APSimpleResDto>>> GetAccountsPayablesAsync(string? keyword, int page, int limit);
    Task<ApiResponse<APComplexResDto>> GetAccountsPayableByIdAsync(int id);
    Task<ApiResponse<APComplexResDto>> AddTransactionAsync(int accountsPayableId, APTransactionCreateReqDto apTransactionCreateReqDto);
}
