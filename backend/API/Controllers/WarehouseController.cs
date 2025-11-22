using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Core.DTOs;
using Core.DTOs.Warehouse;
using Core.Interfaces.Repository;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseController(IWarehouseRepository warehouseRepository) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<WarehouseResDto>>>> GetWarehouses([FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var response = await warehouseRepository.GetWarehousesAsync(keyword, page, limit);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<WarehouseResDto>>> GetWarehouseById(int id)
        {
            var response = await warehouseRepository.GetWarehouseByIdAsync(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<ActionResult<ApiResponse<WarehouseResDto>>> CreateWarehouse([FromBody] WarehouseCreateReqDto warehouseCreateReqDto)
        {
            var response = await warehouseRepository.CreateWarehouseAsync(warehouseCreateReqDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<ActionResult<ApiResponse<WarehouseResDto>>> UpdateWarehouse(int id, [FromBody] WarehouseUpdateReqDto warehouseUpdateReqDto)
        {
            var response = await warehouseRepository.UpdateWarehouseAsync(id, warehouseUpdateReqDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
