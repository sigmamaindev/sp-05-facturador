using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Core.DTOs;
using Core.DTOs.User;
using Core.Interfaces;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserRepository userRepository) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<UserLoginResDto>>> Login([FromBody] UserLoginReqDto loginReqDto)
        {
            var response = await userRepository.LoginAsync(loginReqDto);

            if (!response.Success)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<ActionResult<ApiResponse<List<UserResDto>>>> GetUsers([FromQuery] string? keyword = null, [FromQuery] int page = 1, [FromQuery] int limit = 10)
        {
            var response = await userRepository.GetUsersAsync(keyword, page, limit);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<ActionResult<ApiResponse<UserResDto>>> GetUserById(int id)
        {
            var response = await userRepository.GetUserByIdAsync(id);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<ActionResult<ApiResponse<UserResDto>>> CreateUser([FromBody] UserCreateReqDto userReqDto)
        {
            var response = await userRepository.CreateUserAsync(
                userReqDto,
                userReqDto.RolIds,
                userReqDto.EstablishmentId,
                userReqDto.EmissionPointId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "SuperAdmin, Admin")]
        public async Task<ActionResult<ApiResponse<UserResDto>>> UpdateUser(int id, [FromBody] UserUpdateReqDto userReqDto)
        {
            var response = await userRepository.UpdateUserAsync(
                id,
                userReqDto,
                userReqDto.RolIds,
                userReqDto.EstablishmentId,
                userReqDto.EmissionPointId);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
