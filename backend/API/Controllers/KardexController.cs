using Core.DTOs;
using Core.DTOs.KardexDto;
using Core.Interfaces.Repository;
using Core.Interfaces.Services.IReportService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class KardexController(IKardexRepository kardexRepository, IReportExportService reportExport) : ControllerBase
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

    [HttpGet("report")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<KardexReportWrapperDto>>> GetKardexReport(
        [FromQuery] int productId,
        [FromQuery] DateTime dateFrom,
        [FromQuery] DateTime dateTo)
    {
        var response = await kardexRepository.GetKardexReportAsync(productId, dateFrom, dateTo);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("report/pdf")]
    [Authorize]
    public async Task<IActionResult> GetKardexReportPdf(
        [FromQuery] int productId,
        [FromQuery] DateTime dateFrom,
        [FromQuery] DateTime dateTo)
    {
        var response = await kardexRepository.GetKardexReportAsync(productId, dateFrom, dateTo);

        if (!response.Success || response.Data is null)
        {
            return BadRequest(response);
        }

        var pdfBytes = reportExport.GenerateKardexReportPdf(response.Data);
        return File(pdfBytes, "application/pdf", "ReporteKardex.pdf");
    }

    [HttpGet("report/excel")]
    [Authorize]
    public async Task<IActionResult> GetKardexReportExcel(
        [FromQuery] int productId,
        [FromQuery] DateTime dateFrom,
        [FromQuery] DateTime dateTo)
    {
        var response = await kardexRepository.GetKardexReportAsync(productId, dateFrom, dateTo);

        if (!response.Success || response.Data is null)
        {
            return BadRequest(response);
        }

        var excelBytes = reportExport.GenerateKardexReportExcel(response.Data);
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteKardex.xlsx");
    }
}
