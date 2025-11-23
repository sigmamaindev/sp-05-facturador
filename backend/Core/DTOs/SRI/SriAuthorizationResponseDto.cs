namespace Core.DTOs.SRI;

public class SriAuthorizationResponseDto
{
    public bool Success { get; set; }
    public string State { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string AuthorizationNumber { get; set; } = string.Empty;
    public DateTime? AuthorizationDate { get; set; }
}
