using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Core.DTOs;
using Core.DTOs.Establishment;
using Core.Interfaces;
using Core.Entities;

namespace Infrastructure.Data;

public class EstablishmentRepository(StoreContext context, IHttpContextAccessor httpContextAccessor) : IEstablishmentRepository
{
    public async Task<ApiResponse<EstablishmentResDto>> CreateEstablishmentAsync(EstablishmentReqDto establishmentReqDto)
    {
        var response = new ApiResponse<EstablishmentResDto>();

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

            var existingEstablishment = await context.Establishments
            .FirstOrDefaultAsync(e =>
            e.BusinessId == businessId &&
            e.Name == establishmentReqDto.Name);

            if (existingEstablishment != null)
            {
                response.Success = false;
                response.Message = "El establecimiento ya está registrado en este negocio";
                response.Error = "Error de duplicación";

                return response;
            }

            var lastEstablishment = await context.Establishments
            .Where(e => e.BusinessId == businessId)
            .OrderByDescending(e => e.Code)
            .FirstOrDefaultAsync();

            string newEstablishmentCode;

            if (lastEstablishment == null || string.IsNullOrEmpty(lastEstablishment.Code))
            {
                newEstablishmentCode = "001";
            }
            else
            {
                int lastNumber = int.Parse(lastEstablishment.Code);
                newEstablishmentCode = (lastNumber + 1).ToString().PadLeft(3, '0');
            }

            var newEstablishment = new Establishment
            {
                Code = newEstablishmentCode,
                Name = establishmentReqDto.Name,
                BusinessId = businessId
            };

            context.Establishments.Add(newEstablishment);
            await context.SaveChangesAsync();

            var establishment = new EstablishmentResDto
            {
                Id = newEstablishment.Id,
                Code = newEstablishment.Code,
                Name = newEstablishment.Name,
                IsActive = newEstablishment.IsActive,
                CreatedAt = newEstablishment.CreatedAt
            };

            response.Success = true;
            response.Message = "Establecimiento creado correctamente";
            response.Data = establishment;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al crear el establecimiento";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<EstablishmentResDto>> GetEstablishmentByIdAsync(int id)
    {
        var response = new ApiResponse<EstablishmentResDto>();

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

            var existingEstablishment = await context.Establishments
            .FirstOrDefaultAsync(e =>
            e.BusinessId == businessId &&
            e.Id == id);

            if (existingEstablishment == null)
            {
                response.Success = false;
                response.Message = "Establecimiento no encontrado";
                response.Error = "No existe un establecimiento con el ID especificado";

                return response;
            }

            var establishment = new EstablishmentResDto
            {
                Id = existingEstablishment.Id,
                Code = existingEstablishment.Code,
                Name = existingEstablishment.Name,
                IsActive = existingEstablishment.IsActive,
                CreatedAt = existingEstablishment.CreatedAt
            };

            response.Success = true;
            response.Message = "Establecimiento obtenido correctamente";
            response.Data = establishment;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener el establecimiento";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<List<EstablishmentResDto>>> GetEstablishmentsAsync(string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<EstablishmentResDto>>();

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

            var query = context.Establishments
            .Where(e => e.IsActive && e.BusinessId == businessId);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(
                    e =>
                    EF.Functions.ILike(e.Name, $"%{keyword}%") ||
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
                IsActive = e.IsActive,
                CreatedAt = e.CreatedAt
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

    public async Task<ApiResponse<EstablishmentResDto>> UpdateEstablishmentAsync(int establishmentId, EstablishmentReqDto establishmentReqDto)
    {
        var response = new ApiResponse<EstablishmentResDto>();

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

            var existingEstablishment = await context.Establishments
            .FirstOrDefaultAsync(e =>
            e.BusinessId == businessId &&
            e.Id == establishmentId);

            if (existingEstablishment == null)
            {
                response.Success = false;
                response.Message = "Establecimiento no encontrado";
                response.Error = "No existe el establecimiento especificado";

                return response;
            }

            existingEstablishment.Name = establishmentReqDto.Name;

            await context.SaveChangesAsync();

            var establishment = new EstablishmentResDto
            {
                Id = existingEstablishment.Id,
                Code = existingEstablishment.Code,
                Name = existingEstablishment.Name,
                IsActive = existingEstablishment.IsActive,
                CreatedAt = existingEstablishment.CreatedAt
            };

            response.Success = true;
            response.Message = "Establecimiento actualizado correctamente";
            response.Data = establishment;
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
