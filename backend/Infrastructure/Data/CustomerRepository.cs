using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using Core.DTOs;
using Core.DTOs.Customer;
using Core.DTOs.DocumentType;

namespace Infrastructure.Data;

public class CustomerRepository(StoreContext context, IHttpContextAccessor httpContextAccessor) : ICustomerRepository
{
    public Task<ApiResponse<List<CustomerResDto>>> GetCustomersAsync(string? keyword, int page, int limit)
    {
        throw new NotImplementedException();
    }

    private int GetBusinessIdFromToken()
    {
        var businessIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("BusinessId")?.Value;
        return int.TryParse(businessIdClaim, out var id) ? id : 0;
    }
}
