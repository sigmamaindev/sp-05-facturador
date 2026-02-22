namespace Core.DTOs.CertificateDto;

public class CertificateResDto
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public string? FileName { get; set; }
    public DateTime CreatedAt { get; set; }
}
