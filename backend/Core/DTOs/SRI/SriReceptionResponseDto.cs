using Core.Enums;

namespace Core.DTOs.SRI;

public class SriReceptionResponseDto
{
    public bool Success { get; set; }
    public InvoiceStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
}
