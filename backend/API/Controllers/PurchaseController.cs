using Core.DTOs;
using Core.DTOs.PurchaseDto;
using Core.Interfaces.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PurchaseController(IPurchaseRepository purchaseRepository) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponse<List<PurchaseSimpleResDto>>>> GetPurchases([FromQuery] string? keyword = "", [FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        var response = await purchaseRepository.GetPurchasesAsync(keyword, page, limit);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<PurchaseComplexResDto>>> GetPurchaseById(int id)
    {
        var response = await purchaseRepository.GetPurchaseByIdAsync(id);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ApiResponse<PurchaseSimpleResDto>>> CreatePurchase([FromBody] PurchaseCreateReqDto purchaseCreateReqDto)
    {
        var response = await purchaseRepository.CreatePurchaseAsync(purchaseCreateReqDto);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "SuperAdmin, Admin")]
    public async Task<ActionResult<ApiResponse<PurchaseComplexResDto>>> UpdatePurchase(int id, [FromBody] PurchaseUpdateReqDto purchaseUpdateReqDto)
    {
        var response = await purchaseRepository.UpdatePurchaseAsync(id, purchaseUpdateReqDto);

        if (!response.Success)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}
