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

    [HttpGet("sales/{id:int}/pdf")]
    [Authorize]
    public async Task<IActionResult> GetSalesReportDetailPdf(int id)
    {
        var response = await reportRepository.GetSalesReportDetailAsync(id);
        if (!response.Success || response.Data is null)
        {
            return BadRequest(response);
        }

        var pdfBytes = reportExport.GenerateSalesDetailPdf(response.Data);
        var fileName = $"ReporteVenta_{response.Data.Sequential}.pdf";
        return File(pdfBytes, "application/pdf", fileName);
    }

    [HttpGet("sales/{id:int}/excel")]
    [Authorize]
    public async Task<IActionResult> GetSalesReportDetailExcel(int id)
    {
        var response = await reportRepository.GetSalesReportDetailAsync(id);
        if (!response.Success || response.Data is null)
        {
            return BadRequest(response);
        }

        var excelBytes = reportExport.GenerateSalesDetailExcel(response.Data);
        var fileName = $"ReporteVenta_{response.Data.Sequential}.xlsx";
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
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

    [HttpGet("purchases/{id:int}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<PurchasesReportDetailResDto>>> GetPurchasesReportDetail(int id)
    {
        var response = await reportRepository.GetPurchasesReportDetailAsync(id);
        if (!response.Success)
        {
            return BadRequest(response);
        }
        return Ok(response);
    }

    [HttpGet("purchases/{id:int}/pdf")]
    [Authorize]
    public async Task<IActionResult> GetPurchasesReportDetailPdf(int id)
    {
        var response = await reportRepository.GetPurchasesReportDetailAsync(id);
        if (!response.Success || response.Data is null)
        {
            return BadRequest(response);
        }

        var pdfBytes = reportExport.GeneratePurchasesDetailPdf(response.Data);
        var fileName = $"ReporteCompra_{response.Data.Sequential}.pdf";
        return File(pdfBytes, "application/pdf", fileName);
    }

    [HttpGet("purchases/{id:int}/excel")]
    [Authorize]
    public async Task<IActionResult> GetPurchasesReportDetailExcel(int id)
    {
        var response = await reportRepository.GetPurchasesReportDetailAsync(id);
        if (!response.Success || response.Data is null)
        {
            return BadRequest(response);
        }

        var excelBytes = reportExport.GeneratePurchasesDetailExcel(response.Data);
        var fileName = $"ReporteCompra_{response.Data.Sequential}.xlsx";
        return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}
