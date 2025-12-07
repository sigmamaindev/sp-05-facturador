namespace Core.DTOs.CustomerDto;

public class CustomerUpdateReqDto
{
    public string Document { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? Cellphone { get; set; }
    public string? Telephone { get; set; }
}
