using Core.Interfaces.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CertificateController(ICertificateRepository certificateRepository) : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> UploadCertificate([FromForm] int businessId, [FromForm] IFormFile certificate, [FromForm] string password)
        {
            if (certificate == null)
                return BadRequest("Debe enviar un archivo .p12");

            using var ms = new MemoryStream();
            await certificate.CopyToAsync(ms);
            var bytes = ms.ToArray();

            var response = await certificateRepository.SaveCertificateAsync(businessId, bytes, password);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
