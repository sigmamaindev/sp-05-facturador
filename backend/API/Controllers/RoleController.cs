using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Core.DTOs;
using Core.DTOs.RoleDto;
using Core.Interfaces.Repository;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController(IRoleRepository roleRepository) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<RolResDto>>>> GetRoles()
        {
            var response = await roleRepository.GetRolesAsync();

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
