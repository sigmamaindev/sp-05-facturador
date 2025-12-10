using Core.DTOs;
using Core.DTOs.PurchaseDto;

namespace Core.Interfaces.Purchases;

public interface IPurchaseService
{
    Task<ApiResponse<PurchaseResDto>> CreatePurchaseAsync(PurchaseCreateReqDto purchaseCreateReqDto);
    Task<ApiResponse<PurchaseResDto>> GetPurchaseByIdAsync(int purchaseId);
}
