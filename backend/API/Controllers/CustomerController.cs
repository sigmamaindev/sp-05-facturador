using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.DTOs;
using Core.DTOs.CustomerDto;
using Core.Interfaces.Repository;

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

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<CustomerResDto>>> GetCustomerById(int id)
        {
            var response = await customerRepository.GetCustomerByIdAsync(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<ActionResult<ApiResponse<CustomerResDto>>> CreateCustomer([FromBody] CustomerCreateReqDto customerCreateReqDto)
        {
            var response = await customerRepository.CreateCustomerAsync(customerCreateReqDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<ActionResult<ApiResponse<CustomerResDto>>> UpdateCustomer(int id, [FromBody] CustomerUpdateReqDto customerUpdateReqDto)
        {
            var response = await customerRepository.UpdateCustomerAsync(id, customerUpdateReqDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
