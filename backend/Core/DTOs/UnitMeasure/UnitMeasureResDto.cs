namespace Core.DTOs.UnitMeasure;

public class UnitMeasureResDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal FactorBase { get; set; }
    public bool IsActive { get; set; }
}
