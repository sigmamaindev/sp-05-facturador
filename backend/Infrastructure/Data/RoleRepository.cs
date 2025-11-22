using Microsoft.EntityFrameworkCore;
using Core.DTOs;
using Core.DTOs.Role;
using Core.Interfaces.Repository;

namespace Infrastructure.Data;

public class RoleRepository(StoreContext context) : IRoleRepository
{
    public async Task<ApiResponse<List<RolResDto>>> GetRolesAsync()
    {
        var response = new ApiResponse<List<RolResDto>>();

        try
        {
            var query = context.Roles
            .Where(r => r.Name != "SuperAdmin");

            var roles = await query
            .Select(r => new RolResDto
            {
                Id = r.Id,
                Name = r.Name
            }).ToListAsync();

            response.Success = true;
            response.Message = "Roles obtenidos correctamente";
            response.Data = roles;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener los roles";
            response.Error = ex.Message;
        }

        return response;
    }
}
