using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Core.Interfaces.Repository;
using Core.DTOs;
using Core.DTOs.WarehouseDto;
using Core.Entities;

namespace Infrastructure.Data;

public class WarehouseRepository(StoreContext context, IHttpContextAccessor httpContextAccessor) : IWarehouseRepository
{
    public async Task<ApiResponse<WarehouseResDto>> CreateWarehouseAsync(WarehouseCreateReqDto warehouseCreateReqDto)
    {
        var response = new ApiResponse<WarehouseResDto>();

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

            var existingWarehouse = await context.Warehouses
            .FirstOrDefaultAsync(w =>
            w.BusinessId == businessId &&
            w.Name == warehouseCreateReqDto.Name);

            if (existingWarehouse != null)
            {
                response.Success = false;
                response.Message = "La bodega ya está registrada en este negocio";
                response.Error = "Error de duplicación";

                return response;
            }

            var lastWarehouse = await context.Warehouses
            .Where(w => w.BusinessId == businessId)
            .OrderByDescending(e => e.Code)
            .FirstOrDefaultAsync();

            string newWarehouseCode;

            if (lastWarehouse == null || string.IsNullOrEmpty(lastWarehouse.Code))
            {
                newWarehouseCode = "001";
            }
            else
            {
                int lastNumber = int.Parse(lastWarehouse.Code);
                newWarehouseCode = (lastNumber + 1).ToString().PadLeft(3, '0');
            }

            var newWarehouse = new Warehouse
            {
                Code = newWarehouseCode,
                Name = warehouseCreateReqDto.Name,
                Address = warehouseCreateReqDto.Address
            };

            context.Warehouses.Add(newWarehouse);
            await context.SaveChangesAsync();

            var warehouse = new WarehouseResDto
            {
                Id = newWarehouse.Id,
                Code = newWarehouse.Code,
                Name = newWarehouse.Name,
                Address = newWarehouse.Address,
                IsActive = newWarehouse.IsActive,
                IsMain = newWarehouse.IsMain
            };

            response.Success = true;
            response.Message = "Bodega obtenido correctamente";
            response.Data = warehouse;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener la bodega";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<WarehouseResDto>> GetWarehouseByIdAsync(int id)
    {
        var response = new ApiResponse<WarehouseResDto>();

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

            var existingWarehouse = await context.Warehouses
            .FirstOrDefaultAsync(w =>
            w.BusinessId == businessId &&
            w.Id == id);

            if (existingWarehouse == null)
            {
                response.Success = false;
                response.Message = "Bodega no encontrada";
                response.Error = "No existe una bodega con el ID especificado";

                return response;
            }

            var warehouse = new WarehouseResDto
            {
                Id = existingWarehouse.Id,
                Code = existingWarehouse.Code,
                Name = existingWarehouse.Name,
                Address = existingWarehouse.Address,
                IsActive = existingWarehouse.IsActive,
                IsMain = existingWarehouse.IsMain
            };

            response.Success = true;
            response.Message = "Bodega obtenida correctamente";
            response.Data = warehouse;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener la bodega";
            response.Error = ex.Message;
        }

        return response;
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

    public async Task<ApiResponse<WarehouseResDto>> UpdateWarehouseAsync(int warehouseId, WarehouseUpdateReqDto warehouseUpdateReqDto)
    {
        var response = new ApiResponse<WarehouseResDto>();

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

            var existingWarehouse = await context.Warehouses
            .FirstOrDefaultAsync(w =>
            w.BusinessId == businessId &&
            w.Id == warehouseId);

            if (existingWarehouse == null)
            {
                response.Success = false;
                response.Message = "Establecimiento no encontrado";
                response.Error = "No existe el establecimiento especificado";

                return response;
            }

            existingWarehouse.Name = warehouseUpdateReqDto.Name;
            existingWarehouse.Address = warehouseUpdateReqDto.Address;

            await context.SaveChangesAsync();

            var warehouse = new WarehouseResDto
            {
                Id = existingWarehouse.Id,
                Code = existingWarehouse.Code,
                Address = existingWarehouse.Address,
                Name = existingWarehouse.Name,
                IsActive = existingWarehouse.IsActive,
                IsMain = existingWarehouse.IsMain
            };

            response.Success = true;
            response.Message = "Bodega actualizada correctamente";
            response.Data = warehouse;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al actualizar el establecimiento";
            response.Error = ex.Message;
        }

        return response;
    }

    private int GetBusinessIdFromToken()
    {
        var businessIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("BusinessId")?.Value;
        return int.TryParse(businessIdClaim, out var id) ? id : 0;
    }
}
