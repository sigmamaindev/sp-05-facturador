using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.DTOs;
using Core.DTOs.SupplierDto;
using Core.Interfaces.Repository;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController(ISupplierRepository supplierRepository) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<ActionResult<ApiResponse<SupplierResDto>>> CreateSupplier([FromBody] SupplierCreateReqDto supplierCreateReqDto)
        {
            var response = await supplierRepository.CreateSupplierAsync(supplierCreateReqDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
