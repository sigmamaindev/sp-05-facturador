using Core.DTOs.SRI;

namespace Core.Interfaces.Services;

public interface ISriReceptionService
{
    Task<SriReceptionResponseDto> SendInvoiceSriAsync(string signedXml, bool isProduction);
}
