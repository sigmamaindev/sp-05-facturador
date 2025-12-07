using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Core.Interfaces.Repository;
using Core.DTOs;
using Core.DTOs.EstablishmentDto;

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

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<EstablishmentResDto>>> GetEstablishmentById(int id)
        {
            var response = await establishmentRepository.GetEstablishmentByIdAsync(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<ActionResult<ApiResponse<EstablishmentResDto>>> CreateEstablishment([FromBody] EstablishmentReqDto establishmentReqDto)
        {
            var response = await establishmentRepository.CreateEstablishmentAsync(establishmentReqDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<ActionResult<ApiResponse<EstablishmentResDto>>> UpdateEstablishment(int id, [FromBody] EstablishmentReqDto establishmentReqDto)
        {
            var response = await establishmentRepository.UpdateEstablishmentAsync(id, establishmentReqDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
