using Core.DTOs;
using Core.DTOs.KardexDto;
using Core.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class KardexController(IKardexRepository kardexRepository) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<KardexResDto>>>> GetKardex([FromQuery] string? keyword = "", [FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        var response = await kardexRepository.GetKardexAsync(keyword, page, limit);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
