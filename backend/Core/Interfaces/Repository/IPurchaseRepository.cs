using Core.DTOs;
using Core.DTOs.PurchaseDto;

namespace Core.Interfaces.Repository;

public interface IPurchaseRepository
{
    Task<ApiResponse<List<PurchaseSimpleResDto>>> GetPurchasesAsync(string? keyword, int page, int limit);
    Task<ApiResponse<PurchaseComplexResDto>> GetPurchaseByIdAsync(int id);
    Task<ApiResponse<PurchaseSimpleResDto>> CreatePurchaseAsync(PurchaseCreateReqDto purchaseCreateReqDto);
    Task<ApiResponse<PurchaseComplexResDto>> UpdatePurchaseAsync(int purchaseId, PurchaseUpdateReqDto purchaseUpdateReqDto);
}
