using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Core.DTOs;
using Core.DTOs.Establishment;
using Core.Interfaces;

namespace Infrastructure.Data;

public class EstablishmentRepository(StoreContext context, IHttpContextAccessor httpContextAccessor) : IEstablishmentRepository
{
    public async Task<ApiResponse<List<EstablishmentResDto>>> GetEstablishmentsAsync(string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<EstablishmentResDto>>();

        try
        {
            var businessId = GetBusinessIdFromToken();

            if (businessId == 0)
            {
                response.Success = false;
                response.Message = "No existen establecimientos para este negocio";
                response.Error = "Error de asociación";

                return response;
            }

            var query = context.Establishments
            .Where(e => e.IsActive && e.BusinessId == businessId);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(
                    e =>
                    e.Name.Contains(keyword) ||
                    e.Code.Contains(keyword));
            }

            var total = await query.CountAsync();

            var establishments = await query
            .OrderBy(e => e.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(e => new EstablishmentResDto
            {
                Id = e.Id,
                Code = e.Code,
                Name = e.Name,
                IsActive = e.IsActive
            }).ToListAsync();

            response.Success = true;
            response.Message = "Establecimientos obtenidos correctamente";
            response.Data = establishments;
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
            response.Message = "Error al obtener los establecimientos";
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
