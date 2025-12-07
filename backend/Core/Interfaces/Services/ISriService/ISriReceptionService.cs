using Core.DTOs.SRIDto;

namespace Core.Interfaces.Services.IInvoiceService;

public interface ISriReceptionService
{
    Task<SriReceptionResponseDto> SendInvoiceSriAsync(string signedXml, bool isProduction);
    Task<SriAuthorizationResponseDto> AuthorizeInvoiceSriAsync(string accessKey, bool isProduction);
}
