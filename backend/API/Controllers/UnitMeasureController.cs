using Core.DTOs;
using Core.DTOs.UnitMeasureDto;
using Core.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitMeasureController(IUnitMeasureRepository unitMeasureRepository) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<UnitMeasureResDto>>>> GetUnitMeasures([FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var response = await unitMeasureRepository.GetUnitMeasuresAsync(keyword, page, limit);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UnitMeasureResDto>>> GetUnitMeasureById(int id)
        {
            var response = await unitMeasureRepository.GetUnitMeasureByIdAsync(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
