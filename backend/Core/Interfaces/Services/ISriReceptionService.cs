using Core.DTOs.SRI;

namespace Core.Interfaces.Services;

public interface ISriReceptionService
{
    Task<SriReceptionResponseDto> SendAsync(string signedXml, bool isProduction);
}
