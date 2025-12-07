using Core.Entities;
using Core.DTOs;
using Core.DTOs.BusinessDto;

namespace Core.Interfaces.Repository;

public interface IBusinessRepository
{
    Task<ApiResponse<List<BusinessResDto>>> GetBusinessAsync(string? keyword, int page, int limit);
    Task<ApiResponse<BusinessResDto>> GetBusinessByIdAsync(int id);
}
