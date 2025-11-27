using Core.Enums;

namespace Core.DTOs.SRI;

public class SriAuthorizationResponseDto
{
    public bool Success { get; set; }
    public InvoiceStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public string AuthorizationNumber { get; set; } = string.Empty;
    public DateTime? AuthorizationDate { get; set; }
}
