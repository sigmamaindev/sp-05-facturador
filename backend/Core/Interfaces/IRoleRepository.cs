using Core.DTOs;
using Core.DTOs.Role;

namespace Core.Interfaces;

public interface IRoleRepository
{
    Task<ApiResponse<List<RolResDto>>> GetRolesAsync();
}
