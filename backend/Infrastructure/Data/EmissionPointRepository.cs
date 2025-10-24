using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using Core.DTOs;
using Core.DTOs.EmissionPoint;

namespace Infrastructure.Data;

public class EmissionPointRepository(StoreContext context, IHttpContextAccessor httpContextAccessor) : IEmissionPointRepository
{
    public async Task<ApiResponse<List<EmissionPointResDto>>> GetEmissionPointsAsync(string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<EmissionPointResDto>>();

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

            var establishmentIds = await context.Establishments
            .Where(e => e.IsActive && e.BusinessId == businessId)
            .Select(e => e.Id)
            .ToListAsync();

            if (establishmentIds.Count == 0)
            {
                response.Success = false;
                response.Message = "No existen establecimientos asociados a este negocio";
                response.Error = "Sin establecimientos";

                return response;
            }

            var query = context.EmissionPoints
            .Where(ep => ep.IsActive && establishmentIds.Contains(ep.EstablishmentId));

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(ep =>
                ep.Code.Contains(keyword) ||
                ep.Description.Contains(keyword));
            }

            var total = await query.CountAsync();

            var emissionPoints = await query
            .OrderBy(ep => ep.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(ep => new EmissionPointResDto
            {
                Id = ep.Id,
                Code = ep.Code,
                Description = ep.Description,
                IsActive = ep.IsActive
            }).ToListAsync();

            response.Success = true;
            response.Message = "Puntos de emisión obtenidos correctamente";
            response.Data = emissionPoints;
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
            response.Message = "Error al obtener los puntos de emisión";
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
