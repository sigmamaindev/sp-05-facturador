using Core.DTOs;
using Core.DTOs.EmissionPointDto;
using Core.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmissionPointController(IEmissionPointRepository emissionPointRepository) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<EmissionPointResDto>>>> GetEmissionPoints([FromQuery] int establishmentId = 0, [FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var response = await emissionPointRepository.GetEmissionPointsAsync(establishmentId, keyword, page, limit);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<EmissionPointResDto>>> GetEmissionPointById([FromQuery] int establishmentId, int id)
        {
            var response = await emissionPointRepository.GetEmissionPointByIdAsync(establishmentId, id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<EmissionPointResDto>>> CreateEmissionPoint([FromQuery] int establishmentId, EmissionPointCreateReqDto emissionPointCreateReqDto)
        {
            var response = await emissionPointRepository.CreateEmissionPointAsync(establishmentId, emissionPointCreateReqDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<EmissionPointResDto>>> UpdateEmissionPoint(int id, EmissionPointUpdateReqDto emissionPointUpdateReqDto)
        {
            var response = await emissionPointRepository.UpdateEmissionPointAsync(id, emissionPointUpdateReqDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
