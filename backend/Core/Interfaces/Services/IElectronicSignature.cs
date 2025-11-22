namespace Core.Interfaces.Services;

public interface IElectronicSignature
{
    Task<string> SignXmlAsync(string xmlContent, byte[] certificateBytes, string certificatePassword, CancellationToken cancellationToken = default);
}
