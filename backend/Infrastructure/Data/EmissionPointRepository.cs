using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using Core.DTOs;
using Core.DTOs.EmissionPoint;
using Core.Entities;

namespace Infrastructure.Data;

public class EmissionPointRepository(StoreContext context, IHttpContextAccessor httpContextAccessor) : IEmissionPointRepository
{
    public async Task<ApiResponse<EmissionPointResDto>> CreateEmissionPointAsync(int establishmentId, EmissionPointCreateReqDto emissionPointCreateReqDto)
    {
        var response = new ApiResponse<EmissionPointResDto>();

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

            if (establishmentId == 0)
            {
                response.Success = false;
                response.Message = "Siempre se debe enviar un establecimiento";
                response.Error = "Error de estructura";

                return response;
            }

            var existingEstablishment = await context.Establishments
            .Where(e => e.IsActive &&
             e.BusinessId == businessId &&
             e.Id == establishmentId)
            .FirstOrDefaultAsync();

            if (existingEstablishment == null)
            {
                response.Success = false;
                response.Message = "No existe el establecimiento asociado a este negocio";
                response.Error = "Sin establecimientos";

                return response;
            }

            var lastEmissionPoint = await context.EmissionPoints
            .Where(ep => ep.EstablishmentId == establishmentId)
            .OrderByDescending(ep => ep.Code)
            .FirstOrDefaultAsync();

            string newEmissionPointCode;

            if (lastEmissionPoint == null || string.IsNullOrEmpty(lastEmissionPoint.Code))
            {
                newEmissionPointCode = "001";
            }
            else
            {
                int lastNumber = int.Parse(lastEmissionPoint.Code);
                newEmissionPointCode = (lastNumber + 1).ToString().PadLeft(3, '0');
            }

            var newEmissionPoint = new EmissionPoint
            {
                Code = newEmissionPointCode,
                Description = emissionPointCreateReqDto.Description,
                EstablishmentId = establishmentId
            };

            context.EmissionPoints.Add(newEmissionPoint);
            await context.SaveChangesAsync();

            var emissionPoint = new EmissionPointResDto
            {
                Id = newEmissionPoint.Id,
                Code = newEmissionPoint.Code,
                Description = newEmissionPoint.Description,
                IsActive = newEmissionPoint.IsActive,
                CreatedAt = newEmissionPoint.CreatedAt
            };

            response.Success = true;
            response.Message = "Punto de emisión creado correctamente";
            response.Data = emissionPoint;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al crear el establecimiento";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<EmissionPointResDto>> GetEmissionPointByIdAsync(int establishmentId, int id)
    {
        var response = new ApiResponse<EmissionPointResDto>();

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

            if (establishmentId == 0)
            {
                response.Success = false;
                response.Message = "Siempre se debe enviar un establecimiento";
                response.Error = "Error de estructura";

                return response;
            }

            var existingEstablishment = await context.Establishments
            .Where(e => e.IsActive &&
             e.BusinessId == businessId &&
             e.Id == establishmentId)
            .FirstOrDefaultAsync();

            if (existingEstablishment == null)
            {
                response.Success = false;
                response.Message = "No existe el establecimiento asociado a este negocio";
                response.Error = "Sin establecimientos";

                return response;
            }

            var existingEmissionPoint = await context.EmissionPoints
            .FirstOrDefaultAsync(ep =>
            ep.EstablishmentId == existingEstablishment.Id &&
            ep.Id == id);

            if (existingEmissionPoint == null)
            {
                response.Success = false;
                response.Message = "Punto de emisión no encontrado";
                response.Error = "No existe un punto de emisión con el ID especificado";

                return response;
            }

            var emissionPoint = new EmissionPointResDto
            {
                Id = existingEmissionPoint.Id,
                Code = existingEmissionPoint.Code,
                Description = existingEmissionPoint.Description,
                IsActive = existingEmissionPoint.IsActive,
                CreatedAt = existingEmissionPoint.CreatedAt
            };

            response.Success = true;
            response.Message = "Punto de emisión obtenido correctamente";
            response.Data = emissionPoint;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener el punto de emisión";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<List<EmissionPointResDto>>> GetEmissionPointsAsync(int establishmentId, string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<EmissionPointResDto>>();

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

            if (establishmentId == 0)
            {
                response.Success = false;
                response.Message = "Siempre se debe enviar un establecimiento";
                response.Error = "Error de estructura";

                return response;
            }

            var existingEstablishment = await context.Establishments
            .Where(e => e.IsActive &&
             e.BusinessId == businessId &&
             e.Id == establishmentId)
            .FirstOrDefaultAsync();

            if (existingEstablishment == null)
            {
                response.Success = false;
                response.Message = "No existe el establecimiento asociado a este negocio";
                response.Error = "Sin establecimientos";

                return response;
            }

            var query = context.EmissionPoints
            .Where(ep => ep.IsActive &&
             ep.EstablishmentId == existingEstablishment.Id);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(
                    ep =>
                    EF.Functions.ILike(ep.Description, $"%{keyword}%") ||
                    ep.Code.Contains(keyword));
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
                IsActive = ep.IsActive,
                CreatedAt = ep.CreatedAt
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

    public async Task<ApiResponse<EmissionPointResDto>> UpdateEmissionPointAsync(int emissionPointId, EmissionPointUpdateReqDto emissionPointUpdateReqDto)
    {
        var response = new ApiResponse<EmissionPointResDto>();

        try
        {
            var existingEmissionPoint = await context.EmissionPoints
            .FirstOrDefaultAsync(e =>
            e.Id == emissionPointId);

            if (existingEmissionPoint == null)
            {
                response.Success = false;
                response.Message = "Punto de emission no encontrado";
                response.Error = "No existe el punto de emisión especificado";

                return response;
            }

            existingEmissionPoint.Description = emissionPointUpdateReqDto.Description;
            existingEmissionPoint.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            var emissionPoint = new EmissionPointResDto
            {
                Id = existingEmissionPoint.Id,
                Code = existingEmissionPoint.Code,
                Description = existingEmissionPoint.Description,
                IsActive = existingEmissionPoint.IsActive,
                CreatedAt = existingEmissionPoint.CreatedAt
            };

            response.Success = true;
            response.Message = "Punto de emisión actualizado correctamente";
            response.Data = emissionPoint;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al actualizar el punto de emisión";
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
