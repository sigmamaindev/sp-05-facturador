using Core.DTOs;
using Core.DTOs.PurchaseDto;
using Core.Interfaces.Purchases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/purchases")]
[ApiController]
public class PurchaseController(IPurchaseService purchaseService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public async Task<ActionResult<ApiResponse<PurchaseResDto>>> CreatePurchase([FromBody] PurchaseCreateReqDto purchaseCreateReqDto)
    {
        var response = await purchaseService.CreatePurchaseAsync(purchaseCreateReqDto);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<PurchaseResDto>>> GetPurchaseById(int id)
    {
        var response = await purchaseService.GetPurchaseByIdAsync(id);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}

