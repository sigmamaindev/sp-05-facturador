namespace Core.DTOs.SRI;

public class SriReceptionResponseDto
{
    public bool Success { get; set; }
    public string State { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
