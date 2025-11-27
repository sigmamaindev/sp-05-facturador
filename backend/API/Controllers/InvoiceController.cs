using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Enums;
using Core.DTOs;
using Core.DTOs.Invoice;
using Core.Interfaces.Repository;
using Core.Interfaces.Services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class InvoiceController(IInvoiceRepository invoiceRepository, IInvoicePdfGenerator invoicePdfGenerator) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<InvoiceSimpleResDto>>>> GetInvoices([FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var response = await invoiceRepository.GetInvoicesAsync(keyword, page, limit);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<ActionResult<ApiResponse<InvoiceSimpleResDto>>> CreateInvoice([FromBody] InvoiceCreateReqDto invoiceCreateReqDto)
        {
            var response = await invoiceRepository.CreateInvoiceAsync(invoiceCreateReqDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<InvoiceComplexResDto>>>> GetInvoiceById(int id)
        {
            var response = await invoiceRepository.GetInvoiceByIdAsync(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{id:int}/pdf")]
        [Authorize]
        public async Task<IActionResult> GetInvoicePdf(int id)
        {
            var invoiceResponse = await invoiceRepository.GetInvoiceByIdAsync(id);

            if (!invoiceResponse.Success || invoiceResponse.Data is null)
            {
                return BadRequest(invoiceResponse);
            }

            if (invoiceResponse.Data.Status != InvoiceStatus.SRI_AUTHORIZED)
            {
                return BadRequest(new ApiResponse<string>
                {
                    Success = false,
                    Message = "Factura no autorizada",
                    Error = "Solo las facturas autorizadas pueden generar PDF"
                });
            }

            var pdfBytes = invoicePdfGenerator.Generate(invoiceResponse.Data);
            var fileName = $"Factura_{invoiceResponse.Data.Sequential}.pdf";

            return File(pdfBytes, "application/pdf", fileName);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<InvoiceComplexResDto>>> UpdateInvoiceById(int id, [FromBody] InvoiceUpdateReqDto invoiceUpdateReqDto)
        {
            var response = await invoiceRepository.UpdateInvoiceAsync(id, invoiceUpdateReqDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
