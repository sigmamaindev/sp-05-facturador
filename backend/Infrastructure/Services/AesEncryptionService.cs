using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;
using Core.Interfaces.Services;

namespace Infrastructure.Services;

public class AesEncryptionService(IConfiguration config) : IAesEncryptionService
{
    public string Decrypt(string text)
    {
        var fullBytes = Convert.FromBase64String(text);

        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(EncryptionKey());

        var iv = new byte[aes.BlockSize / 8];
        var cipherBytes = new byte[fullBytes.Length - iv.Length];

        Buffer.BlockCopy(fullBytes, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(fullBytes, iv.Length, cipherBytes, 0, cipherBytes.Length);

        aes.IV = iv;

        using var decryptor = aes.CreateDecryptor();
        var decryptedBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);

        return Encoding.UTF8.GetString(decryptedBytes);
    }

    public string Encrypt(string text)
    {
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(EncryptionKey());
        aes.GenerateIV();

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(text);

        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        var result = new byte[aes.IV.Length + cipherBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, aes.IV.Length, cipherBytes.Length);

        return Convert.ToBase64String(result);
    }

    private string EncryptionKey()
    {
        var key = config["EncryptionKey"];

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new Exception("EncryptionKey is missing in appsettings.json");
        }

        if (key.Length != 32)
        {
            throw new Exception("EncryptionKey must be exactly 32 characters for AES-256.");
        }

        return key;
    }
}
