using Core.DTOs;
using Core.DTOs.Warehouse;

namespace Core.Interfaces;

public interface IWarehouseRepository
{
    Task<ApiResponse<List<WarehouseResDto>>> GetWarehousesAsync(string? keyword, int page, int limit);
    Task<ApiResponse<WarehouseResDto>> GetWarehouseByIdAsync(int id);
    Task<ApiResponse<WarehouseResDto>> CreateWarehouseByIdAsync(WarehouseCreateReqDto warehouseCreateReqDto);
    Task<ApiResponse<WarehouseResDto>> UpdateWarehouseByIdAsync(int warehouseId, WarehouseCreateReqDto warehouseCreateReqDto);
}
