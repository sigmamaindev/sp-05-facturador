using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Core.DTOs;
using Core.DTOs.Invoice;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController(IInvoiceRepository invoiceRepository) : ControllerBase
    {
        [HttpPost("draft")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<InvoiceResDto>>> CreateIDraftInvoice([FromBody] InvoiceCreateReqDto invoiceCreateReqDto)
        {
            var response = await invoiceRepository.CreateDraftInvoiceAsync(invoiceCreateReqDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
