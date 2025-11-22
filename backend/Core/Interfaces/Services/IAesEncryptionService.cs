namespace Core.Interfaces.Services;

public interface IAesEncryptionService
{
    string Encrypt(string text);
    string Decrypt(string text);
}
