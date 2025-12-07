namespace Core.DTOs.ProductDto;

public class ProductUpdateReqDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool Iva { get; set; }
    public string Type { get; set; } = string.Empty;
    public int UnitMeasureId { get; set; }
    public int TaxId { get; set; }
}
