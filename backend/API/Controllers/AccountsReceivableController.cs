using Core.DTOs;
using Core.DTOs.ARDto;
using Core.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsReceivableController(IAccountsReceivableRepository accountsReceivableRepository) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<ARSimpleResDto>>>> GetAccountsReceivables([FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        var response = await accountsReceivableRepository.GetAccountsReceivablesAsync(keyword, page, limit);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("by-customer")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<ARCustomerSummaryResDto>>>> GetAccountsReceivablesByCustomer([FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        var response = await accountsReceivableRepository.GetAccountsReceivablesByCustomerAsync(keyword, page, limit);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("customer/{customerId:int}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<ARSimpleResDto>>>> GetAccountsReceivablesByCustomerId(int customerId, [FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        var response = await accountsReceivableRepository.GetAccountsReceivablesByCustomerIdAsync(customerId, keyword, page, limit);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ARComplexResDto>>> GetARById(int id)
    {
        var response = await accountsReceivableRepository.GetAccountsReceivableByIdAsync(id);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("{id:int}/transactions")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<ARComplexResDto>>> AddTransaction(int id, [FromBody] ARTransactionCreateReqDto aRTransactionCreateReqDto)
    {
        var response = await accountsReceivableRepository.AddTransactionAsync(id, aRTransactionCreateReqDto);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("payments")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<ARSimpleResDto>>>> AddBulkPayments([FromBody] ARBulkPaymentCreateReqDto request)
    {
        var response = await accountsReceivableRepository.AddBulkPaymentsAsync(request);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
