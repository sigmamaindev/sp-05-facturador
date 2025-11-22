using Core.DTOs;
using Core.DTOs.Inventory;

namespace Core.Interfaces.Repository;

public interface IInventoryRepository
{
    Task<ApiResponse<List<InventoryResDto>>> CreateInventoryByProductIdAsync(int productId, InventoryCreateReqDto inventoryCreateReqDto);
}
