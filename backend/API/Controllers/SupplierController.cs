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
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<SupplierResDto>>>> GetSuppliers([FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var response = await supplierRepository.GetSuppliersAsync(keyword, page, limit);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<SupplierResDto>>> GetSupplierById(int id)
        {
            var response = await supplierRepository.GetSupplierByIdAsync(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

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

        [HttpPut("{id:int}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<ActionResult<ApiResponse<SupplierResDto>>> UpdateSupplier(int id, [FromBody] SupplierUpdateReqDto supplierUpdateReqDto)
        {
            var response = await supplierRepository.UpdateSupplierAsync(id, supplierUpdateReqDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
