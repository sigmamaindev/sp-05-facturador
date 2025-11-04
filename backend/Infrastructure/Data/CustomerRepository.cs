using Microsoft.AspNetCore.Http;
using Core.Interfaces;
using Core.DTOs;
using Core.DTOs.Customer;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class CustomerRepository(StoreContext context, IHttpContextAccessor httpContextAccessor) : ICustomerRepository
{
    public async Task<ApiResponse<List<CustomerResDto>>> GetCustomersAsync(string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<CustomerResDto>>();

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

            var query = context.Customers
            .Where(c => c.IsActive && c.BusinessId == businessId);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(
                    c =>
                    EF.Functions.ILike(c.Document, $"%{keyword}%") ||
                    EF.Functions.ILike(c.Name, $"%{keyword}%") ||
                    EF.Functions.ILike(c.Address, $"%{keyword}%"));
            }

            var total = await query.CountAsync();

            var customers = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(c => new CustomerResDto
            {
                Id = c.Id,
                Document = c.Document,
                Name = c.Name,
                Email = c.Email,
                Address = c.Address,
                Cellphone = c.Cellphone,
                Telephone = c.Telephone,
                IsActive = c.IsActive,
                DocumentType = c.DocumentType,
                CreatedAt = c.CreatedAt
            }).ToListAsync();

            response.Success = true;
            response.Message = "Clientes obtenidos correctamente";
            response.Data = customers;
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
            response.Message = "Error al obtener los clientes";
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
