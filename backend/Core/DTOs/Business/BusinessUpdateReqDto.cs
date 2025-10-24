namespace Core.DTOs.Business;

public class BusinessUpdateReqDto
{
    public required string Document { get; set; }
    public required string Name { get; set; }
    public required string Address { get; set; }
    public string? City { get; set; }
    public string? Province { get; set; }
    public bool IsActive { get; set; }
}
