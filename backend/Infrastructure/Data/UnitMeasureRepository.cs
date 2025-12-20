using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Core.DTOs;
using Core.Interfaces.Repository;
using Core.DTOs.UnitMeasureDto;

namespace Infrastructure.Data;

public class UnitMeasureRepository(StoreContext context, IHttpContextAccessor httpContextAccessor) : IUnitMeasureRepository
{
    public async Task<ApiResponse<List<UnitMeasureResDto>>> GetUnitMeasuresAsync(string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<UnitMeasureResDto>>();
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

            var query = context.UnitMeasures
            .Where(um => um.IsActive && um.BusinessId == businessId);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(
                    um =>
                    EF.Functions.ILike(um.Code, $"%{keyword}%") ||
                    EF.Functions.ILike(um.Name, $"%{keyword}%"));
            }

            var total = await query.CountAsync();

            var unitMeasures = await query
            .OrderBy(um => um.Code)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(um => new UnitMeasureResDto
            {
                Id = um.Id,
                Code = um.Code,
                Name = um.Name,
                FactorBase = um.FactorBase,
                IsActive = um.IsActive
            }).ToListAsync();

            response.Success = true;
            response.Message = "Unidades de medida obtenidas correctamente";
            response.Data = unitMeasures;
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
            response.Message = "Error al obtener las unidades de medida";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<UnitMeasureResDto>> GetUnitMeasureByIdAsync(int id)
    {
        var response = new ApiResponse<UnitMeasureResDto>();

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

            var existingUnitMeasure = await context.UnitMeasures
            .FirstOrDefaultAsync(
                um =>
                um.Id == id &&
                um.BusinessId == businessId &&
                um.IsActive);

            if (existingUnitMeasure == null)
            {
                response.Success = false;
                response.Message = "Unidad de medida no encontrada";
                response.Error = "No existe una unidad de medida con el ID especificado";

                return response;
            }

            var unitMeasure = new UnitMeasureResDto
            {
                Id = existingUnitMeasure.Id,
                Code = existingUnitMeasure.Code,
                Name = existingUnitMeasure.Name,
                FactorBase = existingUnitMeasure.FactorBase,
                IsActive = existingUnitMeasure.IsActive
            };

            response.Success = true;
            response.Message = "Unidad de medida obtenida correctamente";
            response.Data = unitMeasure;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener la unidad de medida";
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
