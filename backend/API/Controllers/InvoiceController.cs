using Core.DTOs;
using Core.DTOs.Invoice;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class InvoiceController(IInvoiceRepository invoiceRepository) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<InvoiceResDto>>>> GetInvoices([FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int limit = 10)
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
        public async Task<ActionResult<ApiResponse<InvoiceResDto>>> CreateInvoice([FromBody] InvoiceCreateReqDto invoiceCreateReqDto)
        {
            var response = await invoiceRepository.CreateInvoiceAsync(invoiceCreateReqDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
