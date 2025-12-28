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
}

