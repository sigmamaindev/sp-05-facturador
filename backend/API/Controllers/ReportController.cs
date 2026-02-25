using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.DTOs;
using Core.DTOs.ReportDto;
using Core.Interfaces.Repository;
using Core.Interfaces.Services.IReportService;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReportController(IReportRepository reportRepository, IReportExportService reportExport) : ControllerBase
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

    [HttpGet("sales/pdf")]
    [Authorize]
    public async Task<IActionResult> GetSalesReportPdf(
        [FromQuery] string? keyword = null,
        [FromQuery] int? creditDays = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        var response = await reportRepository.GetSalesReportAsync(keyword, creditDays, dateFrom, dateTo, 1, int.MaxValue);
        if (!response.Success || response.Data is null)
        {
            return BadRequest(response);
        }

        var pdfBytes = reportExport.GenerateSalesReportPdf(response.Data);
        return File(pdfBytes, "application/pdf", "ReporteVentas.pdf");
    }

    [HttpGet("sales/excel")]
    [Authorize]
    public async Task<IActionResult> GetSalesReportExcel(
        [FromQuery] string? keyword = null,
        [FromQuery] int? creditDays = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        var response = await reportRepository.GetSalesReportAsync(keyword, creditDays, dateFrom, dateTo, 1, int.MaxValue);
        if (!response.Success || response.Data is null)
        {
            return BadRequest(response);
        }

        var excelBytes = reportExport.GenerateSalesReportExcel(response.Data);
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteVentas.xlsx");
    }

    // ─── Compras ────────────────────────────────────────────────────────────

    [HttpGet("purchases")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<PurchasesReportResDto>>>> GetPurchasesReport(
        [FromQuery] string? keyword = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] int page = 1,
        [FromQuery] int limit = 10)
    {
        var response = await reportRepository.GetPurchasesReportAsync(keyword, dateFrom, dateTo, page, limit);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpGet("purchases/pdf")]
    [Authorize]
    public async Task<IActionResult> GetPurchasesReportPdf(
        [FromQuery] string? keyword = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        var response = await reportRepository.GetPurchasesReportAsync(keyword, dateFrom, dateTo, 1, int.MaxValue);
        if (!response.Success || response.Data is null)
        {
            return BadRequest(response);
        }

        var pdfBytes = reportExport.GeneratePurchasesReportPdf(response.Data);
        return File(pdfBytes, "application/pdf", "ReporteCompras.pdf");
    }

    [HttpGet("purchases/excel")]
    [Authorize]
    public async Task<IActionResult> GetPurchasesReportExcel(
        [FromQuery] string? keyword = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null)
    {
        var response = await reportRepository.GetPurchasesReportAsync(keyword, dateFrom, dateTo, 1, int.MaxValue);
        if (!response.Success || response.Data is null)
        {
            return BadRequest(response);
        }

        var excelBytes = reportExport.GeneratePurchasesReportExcel(response.Data);
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteCompras.xlsx");
    }
}
