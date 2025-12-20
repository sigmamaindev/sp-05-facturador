using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces.Repository;
using Core.DTOs;
using Core.DTOs.BusinessDto;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessController(IBusinessRepository businessRepository) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<BusinessResDto>>>> GetBusiness([FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var response = await businessRepository.GetBusinessAsync(keyword, page, limit);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<BusinessResDto>>>> GetBusinessById(int id)
        {
            var response = await businessRepository.GetBusinessByIdAsync(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<ActionResult<ApiResponse<BusinessResDto>>> UpdateCustomer(int id, [FromBody] BusinessUpdateReqDto businessUpdateReqDto)
        {
            var response = await businessRepository.UpdateBusinessAsync(id, businessUpdateReqDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
