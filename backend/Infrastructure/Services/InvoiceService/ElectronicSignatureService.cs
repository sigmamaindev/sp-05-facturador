using Yamgooo.SRI.Sign;
using Yamgooo.SRI.Sign.Result;
using Core.Interfaces.Services.IInvoiceService;

namespace Infrastructure.Services.InvoiceService;

public class ElectronicSignatureService(ISriSignService sriSignService) : IElectronicSignatureService
{
    public async Task<string> SignXmlAsync(
        string xmlContent,
        byte[] pfxBytes,
        string pfxPassword,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(xmlContent);
        ArgumentNullException.ThrowIfNull(pfxBytes);
        ArgumentException.ThrowIfNullOrWhiteSpace(pfxPassword);

        var certificateBase64 = Convert.ToBase64String(pfxBytes);

        var isValid = sriSignService.ValidateBase64Certificate(certificateBase64, pfxPassword);
        if (!isValid)
        {
            throw new InvalidOperationException("El certificado Base64 del SRI es inválido o la contraseña es incorrecta.");
        }

        SignatureResult result = await sriSignService.SignWithBase64CertificateAsync(
            xmlContent,
            certificateBase64,
            pfxPassword
        );

        if (!result.Success)
        {
            throw new InvalidOperationException($"Error al firmar el XML SRI: {result.ErrorMessage}");
        }

        return result.SignedXml;
    }
}
