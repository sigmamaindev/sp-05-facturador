using Core.DTOs;
using Core.DTOs.ARDto;

namespace Core.Interfaces.Repository;

public interface IAccountsReceivableRepository
{
    Task<ApiResponse<List<ARSimpleResDto>>> GetAccountsReceivablesAsync(string? keyword, int page, int limit);
    Task<ApiResponse<List<ARCustomerSummaryResDto>>> GetAccountsReceivablesByCustomerAsync(string? keyword, int page, int limit);
    Task<ApiResponse<List<ARSimpleResDto>>> GetAccountsReceivablesByCustomerIdAsync(int customerId, string? keyword, int page, int limit);
    Task<ApiResponse<List<ARSimpleResDto>>> AddBulkPaymentsAsync(ARBulkPaymentCreateReqDto request);
    Task<ApiResponse<ARComplexResDto>> GetAccountsReceivableByIdAsync(int id);
    Task<ApiResponse<ARComplexResDto>> AddTransactionAsync(int accountsReceivableId, ARTransactionCreateReqDto aRTransactionCreateReqDto);
}
