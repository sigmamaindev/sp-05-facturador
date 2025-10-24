using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using Core.DTOs;
using Core.DTOs.Customer;
using Core.DTOs.Business;
using Core.DTOs.DocumentType;

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
                response.Message = "Negocios no asociados al contexto actual";
                response.Error = "Error de asociación";

                return response;
            }

            var query = context.Customers
            .Include(c => c.Business)
            .Include(c => c.DocumentType)
            .Where(c => c.IsActive && c.BusinessId == businessId);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(
                    c =>
                    c.Document.Contains(keyword) ||
                    c.Name.Contains(keyword) ||
                    c.Email.Contains(keyword));
            }

            var total = await query.CountAsync();

            var customers = await query
            .OrderBy(c => c.Name)
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
                DocumentType = new DocumentTypeResDto
                {
                    Id = c.DocumentType!.Id,
                    Code = c.DocumentType.Code,
                    Name = c.DocumentType.Name
                }
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
