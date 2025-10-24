using Core.DTOs;
using Core.DTOs.Customer;

namespace Core.Interfaces;

public interface ICustomerRepository
{
    Task<ApiResponse<List<CustomerResDto>>> GetCustomersAsync(string? keyword, int page, int limit);
}
