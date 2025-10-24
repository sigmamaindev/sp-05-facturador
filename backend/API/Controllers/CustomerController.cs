using Core.DTOs;
using Core.DTOs.Customer;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController(ICustomerRepository customerRepository) : ControllerBase
    {
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<CustomerResDto>>>> GetCustomers([FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var response = await customerRepository.GetCustomersAsync(keyword, page, limit);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
