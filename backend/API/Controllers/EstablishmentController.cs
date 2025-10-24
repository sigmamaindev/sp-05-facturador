using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Core.Interfaces;
using Core.DTOs;
using Core.DTOs.Establishment;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstablishmentController(IEstablishmentRepository establishmentRepository) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<EstablishmentResDto>>>> GetEstablishments([FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var response = await establishmentRepository.GetEstablishmentsAsync(keyword, page, limit);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
