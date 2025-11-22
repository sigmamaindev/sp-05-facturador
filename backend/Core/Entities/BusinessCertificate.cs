namespace Core.Entities;

public class BusinessCertificate : BaseEntity
{
    public int BusinessId { get; set; }
    public Business? Business { get; set; }
    public required string CertificateBase64 { get; set; }
    public required string Password { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
