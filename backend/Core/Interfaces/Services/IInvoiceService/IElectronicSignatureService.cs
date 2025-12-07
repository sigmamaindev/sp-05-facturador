namespace Core.Interfaces.Services.IInvoiceService;

public interface IElectronicSignatureService
{
    Task<string> SignXmlAsync(string xmlContent, byte[] certificateBytes, string certificatePassword, CancellationToken cancellationToken = default);
}
