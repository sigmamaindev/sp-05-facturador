namespace Core.DTOs.Product;

public class ProductCreateReqDto
{
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool Iva { get; set; }
    public string Type { get; set; } = string.Empty;
    public int UnitMeasureId { get; set; }
    public int TaxId { get; set; }
}
