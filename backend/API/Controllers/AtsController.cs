using System.Text;
using Core.DTOs;
using Core.DTOs.AtsDto;
using Core.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AtsController(IAtsRepository atsRepository) : ControllerBase
{
    [HttpGet("purchases")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<AtsPurchaseResDto>>>> GetAtsPurchases([FromQuery] int year, [FromQuery] int month)
    {
        var response = await atsRepository.GetAtsPurchasesAsync(year, month);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("sales")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<AtsSaleResDto>>>> GetAtsSales([FromQuery] int year, [FromQuery] int month)
    {
        var response = await atsRepository.GetAtsSalesAsync(year, month);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("xml")]
    [Authorize]
    public async Task<IActionResult> GetAtsXml([FromQuery] int year, [FromQuery] int month)
    {
        var response = await atsRepository.GetAtsXmlAsync(year, month);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        var xml = response.Data ?? string.Empty;
        var fileName = $"{month:D2}{year}.xml";
        return File(Encoding.UTF8.GetBytes(xml), "application/xml", fileName);
    }
}
