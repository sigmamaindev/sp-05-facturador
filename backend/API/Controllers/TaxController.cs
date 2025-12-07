using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces.Repository;
using Core.DTOs;
using Core.DTOs.TaxDto;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaxController(ITaxRepository taxRepository) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<TaxResDto>>>> GetTaxes([FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var response = await taxRepository.GetTaxesAsync(keyword, page, limit);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
