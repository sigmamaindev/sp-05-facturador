using Core.DTOs;
using Core.DTOs.Warehouse;

namespace Core.Interfaces.Repository;

public interface IWarehouseRepository
{
    Task<ApiResponse<List<WarehouseResDto>>> GetWarehousesAsync(string? keyword, int page, int limit);
    Task<ApiResponse<WarehouseResDto>> GetWarehouseByIdAsync(int id);
    Task<ApiResponse<WarehouseResDto>> CreateWarehouseAsync(WarehouseCreateReqDto warehouseCreateReqDto);
    Task<ApiResponse<WarehouseResDto>> UpdateWarehouseAsync(int warehouseId, WarehouseUpdateReqDto warehouseUpdateReqDto);
}
