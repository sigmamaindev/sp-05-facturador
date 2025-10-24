namespace Core.DTOs.Tax;

public class TaxResDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string CodePercentage { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Group { get; set; }
    public decimal Rate { get; set; }
    public bool IsActive { get; set; }
}
