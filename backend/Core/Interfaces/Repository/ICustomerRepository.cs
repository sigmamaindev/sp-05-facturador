using Core.DTOs;
using Core.DTOs.CustomerDto;

namespace Core.Interfaces.Repository;

public interface ICustomerRepository
{
    Task<ApiResponse<List<CustomerResDto>>> GetCustomersAsync(string? keyword, int page, int limit);
    Task<ApiResponse<CustomerResDto>> GetCustomerByIdAsync(int id);
    Task<ApiResponse<CustomerResDto>> CreateCustomerAsync(CustomerCreateReqDto customerCreateReqDto);
    Task<ApiResponse<CustomerResDto>> UpdateCustomerAsync(int customerId, CustomerUpdateReqDto customerUpdateReqDto);
}
