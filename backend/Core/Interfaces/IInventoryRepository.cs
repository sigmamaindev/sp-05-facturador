using Core.DTOs;
using Core.DTOs.Inventory;

namespace Core.Interfaces;

public interface IInventoryRepository
{
    Task<ApiResponse<List<InventoryResDto>>> CreateInventoryByProductIdAsync(int productId, InventoryCreateReqDto inventoryCreateReqDto);
}
