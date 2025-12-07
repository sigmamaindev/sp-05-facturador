namespace Core.Interfaces.Services.IUtilService;

public interface IAesEncryptionService
{
    string Encrypt(string text);
    string Decrypt(string text);
}
