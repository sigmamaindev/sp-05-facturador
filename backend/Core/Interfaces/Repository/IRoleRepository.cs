using Core.DTOs;
using Core.DTOs.RoleDto;

namespace Core.Interfaces.Repository;

public interface IRoleRepository
{
    Task<ApiResponse<List<RolResDto>>> GetRolesAsync();
}
