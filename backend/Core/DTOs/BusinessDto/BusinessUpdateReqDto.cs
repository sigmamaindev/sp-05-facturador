namespace Core.DTOs.BusinessDto;

public class BusinessUpdateReqDto
{
    public string Document { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Province { get; set; } = string.Empty;
    public string SriEnvironment { get; set; } = string.Empty;
}
