using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.DTOs;
using Core.DTOs.ReportDto;
using Core.Interfaces.Repository;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportController(IReportRepository reportRepository) : ControllerBase
{
    [HttpGet("sales")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<SalesReportResDto>>>> GetSalesReport(
        [FromQuery] string? keyword = null,
        [FromQuery] int? creditDays = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10)
    {
        var response = await reportRepository.GetSalesReportAsync(keyword, creditDays, dateFrom, dateTo, page, limit);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpGet("sales/{id:int}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<SalesReportDetailResDto>>> GetSalesReportDetail(int id)
    {
        var response = await reportRepository.GetSalesReportDetailAsync(id);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
}
