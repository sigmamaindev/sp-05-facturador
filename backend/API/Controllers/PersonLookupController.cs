using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Core.DTOs;
using Core.DTOs.PersonLookupDto;
using Core.Interfaces.Repository;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonLookupController(IPersonLookupRepository personLookupRepository) : ControllerBase
    {
        [HttpGet("{document}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PersonLookupResDto>>> LookupByDocument(string document)
        {
            var response = await personLookupRepository.LookupByDocumentAsync(document);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
