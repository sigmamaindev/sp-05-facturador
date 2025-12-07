using Microsoft.AspNetCore.Http;
using Core.Interfaces.Repository;
using Core.DTOs;
using Core.DTOs.TaxDto;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class TaxRepository(StoreContext context, IHttpContextAccessor httpContextAccessor) : ITaxRepository
{
    public async Task<ApiResponse<List<TaxResDto>>> GetTaxesAsync(string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<TaxResDto>>();

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

            var query = context.Taxes
            .Where(t => t.IsActive && t.BusinessId == businessId);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(
                    um =>
                    EF.Functions.ILike(um.Name, $"%{keyword}%"));
            }

            var total = await query.CountAsync();

            var taxes = await query
            .OrderBy(t => t.Name)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(t => new TaxResDto
            {
                Id = t.Id,
                Code = t.Code,
                CodePercentage = t.CodePercentage,
                Name = t.Name,
                Group = t.Group,
                Rate = t.Rate,
                IsActive = t.IsActive
            }).ToListAsync();

            response.Success = true;
            response.Message = "Impuestos obtenidos correctamente";
            response.Data = taxes;
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
            response.Message = "Error al obtener los impuestos";
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
