using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces.Repository;
using Core.DTOs;
using Core.DTOs.InventoryDto;
using Core.DTOs.ProductDto;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProductRepository productRepository, IInventoryRepository inventoryRepository) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<ProductResDto>>>> GetProducts([FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var response = await productRepository.GetProductsAsync(keyword, page, limit);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<ActionResult<ApiResponse<ProductResDto>>> CreateProduct([FromBody] ProductCreateReqDto productCreateReqDto)
        {
            var response = await productRepository.CreateProductAsync(productCreateReqDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ProductResDto>>> GetProductById(int id)
        {
            var response = await productRepository.GetProductByIdAsync(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<ActionResult<ApiResponse<ProductResDto>>> UpdateProduct(int id, [FromBody] ProductUpdateReqDto productUpdateReqDto)
        {
            var response = await productRepository.UpdateProductAsync(id, productUpdateReqDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("{id:int}/inventory")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<ActionResult<ApiResponse<InventoryResDto>>> UpdateProductInventory(int id, [FromBody] InventoryUpdateReqDto inventoryUpdateReqDto)
        {
            var response = await inventoryRepository.UpdateInventoryByProductIdAsync(id, inventoryUpdateReqDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
