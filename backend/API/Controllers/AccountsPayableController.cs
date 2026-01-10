using Core.DTOs;
using Core.DTOs.APDto;
using Core.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsPayableController(IAccountsPayableRepository accountsPayableRepository) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<APSimpleResDto>>>> GetAccountsPayables([FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        var response = await accountsPayableRepository.GetAccountsPayablesAsync(keyword, page, limit);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<APComplexResDto>>> GetAPById(int id)
    {
        var response = await accountsPayableRepository.GetAccountsPayableByIdAsync(id);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost("{id:int}/transactions")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<APComplexResDto>>> AddTransaction(int id, [FromBody] APTransactionCreateReqDto apTransactionCreateReqDto)
    {
        var response = await accountsPayableRepository.AddTransactionAsync(id, apTransactionCreateReqDto);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
