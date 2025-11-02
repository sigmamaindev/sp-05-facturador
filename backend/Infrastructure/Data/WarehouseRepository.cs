using Microsoft.AspNetCore.Http;
using Core.Interfaces;
using Core.DTOs;
using Core.DTOs.Warehouse;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class WarehouseRepository(StoreContext context, IHttpContextAccessor httpContextAccessor) : IWarehouseRepository
{
    public Task<ApiResponse<WarehouseResDto>> CreateWarehouseByIdAsync(WarehouseCreateReqDto warehouseCreateReqDto)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<WarehouseResDto>> GetWarehouseByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResponse<List<WarehouseResDto>>> GetWarehousesAsync(string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<WarehouseResDto>>();

        try
        {
            var businessId = GetBusinessIdFromToken();

            if (businessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no asociado a esta usuario";
                response.Error = "Error de asociación";

                return response;
            }

            var query = context.Warehouses
            .Where(w => w.IsActive && w.BusinessId == businessId);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(
                    w =>
                    EF.Functions.Like(w.Name, $"%{keyword}%") ||
                    EF.Functions.Like(w.Address, $"%{keyword}%") ||
                    EF.Functions.Like(w.Code, $"%{keyword}%"));
            }

            var total = await query.CountAsync();

            var warehouses = await query
            .OrderBy(w => w.Code)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(w => new WarehouseResDto
            {
                Id = w.Id,
                Code = w.Code,
                Name = w.Name,
                Address = w.Address,
                IsActive = w.IsActive,
                IsMain = w.IsMain
            }).ToListAsync();

            response.Success = true;
            response.Message = "Bodegas obtenidas correctamente";
            response.Data = warehouses;
            response.Pagination = new Pagination
            {
                Total = total,
                Page = page,
                Limit = limit
            };
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener las bodegas";
            response.Error = ex.Message;
        }

        return response;
    }

    public Task<ApiResponse<WarehouseResDto>> UpdateWarehouseByIdAsync(int warehouseId, WarehouseCreateReqDto warehouseCreateReqDto)
    {
        throw new NotImplementedException();
    }

    private int GetBusinessIdFromToken()
    {
        var businessIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("BusinessId")?.Value;
        return int.TryParse(businessIdClaim, out var id) ? id : 0;
    }
}
