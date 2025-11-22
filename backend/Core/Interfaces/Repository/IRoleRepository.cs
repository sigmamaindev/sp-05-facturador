using Core.DTOs;
using Core.DTOs.Role;

namespace Core.Interfaces.Repository;

public interface IRoleRepository
{
    Task<ApiResponse<List<RolResDto>>> GetRolesAsync();
}
