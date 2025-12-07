using Core.Interfaces.Repository;
using Core.DTOs;
using Core.DTOs.BusinessDto;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class BusinessRepository(StoreContext context) : IBusinessRepository
{
    public async Task<ApiResponse<List<BusinessResDto>>> GetBusinessAsync(string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<BusinessResDto>>();

        try
        {
            var query = context.Businesses.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(
                b =>
                b.Document.Contains(keyword) ||
                b.Name.Contains(keyword));
            }

            var total = await query.CountAsync();

            var business = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(b => new BusinessResDto
            {
                Id = b.Id,
                Document = b.Document,
                Name = b.Name,
                Address = b.Address,
                City = b.City ?? string.Empty,
                Province = b.Province ?? string.Empty,
                CreatedAt = b.CreatedAt,
                IsActive = b.IsActive
            }).ToListAsync();

            response.Success = true;
            response.Message = "Empresas obtenidas correctamente";
            response.Data = business;
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
            response.Message = "Error al obtener las empresas";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<BusinessResDto>> GetBusinessByIdAsync(int id)
    {
        var response = new ApiResponse<BusinessResDto>();

        try
        {
            var existingBusiness = await context.Businesses
            .FirstOrDefaultAsync(b => b.Id == id);

            if (existingBusiness == null)
            {
                response.Success = false;
                response.Message = "Empresa no encontrada";
                response.Error = "No existe una empresa con el ID especificado";

                return response;
            }

            var business = new BusinessResDto
            {
                Id = existingBusiness.Id,
                Document = existingBusiness.Document,
                Name = existingBusiness.Name,
                Address = existingBusiness.Address,
                City = existingBusiness.City!,
                Province = existingBusiness.Province!,
                IsActive = existingBusiness.IsActive,
                CreatedAt = existingBusiness.CreatedAt
            };

            response.Success = true;
            response.Message = "Empresa obtenida correctamente";
            response.Data = business;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener la empresa";
            response.Error = ex.Message;
        }

        return response;
    }
}
