using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces.Repository;
using Core.DTOs;
using Core.DTOs.InventoryDto;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController(IInventoryRepository inventoryRepository) : ControllerBase
    {
        [HttpPost("{id:int}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<ActionResult<ApiResponse<InventoryResDto>>> CreateInventory(int id, [FromBody] InventoryCreateReqDto inventoryCreateReqDto)
        {
            var response = await inventoryRepository.CreateInventoryByProductIdAsync(id, inventoryCreateReqDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
