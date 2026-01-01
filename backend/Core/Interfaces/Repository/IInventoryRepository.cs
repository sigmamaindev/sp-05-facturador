using Core.DTOs;
using Core.DTOs.InventoryDto;

namespace Core.Interfaces.Repository;

public interface IInventoryRepository
{
    Task<ApiResponse<List<InventoryResDto>>> CreateInventoryByProductIdAsync(int productId, InventoryCreateReqDto inventoryCreateReqDto);
    Task<ApiResponse<InventoryResDto>> UpdateInventoryByProductIdAsync(int productId, InventoryUpdateReqDto inventoryUpdateReqDto);
}
