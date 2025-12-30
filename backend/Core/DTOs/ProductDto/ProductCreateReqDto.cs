namespace Core.DTOs.ProductDto;

public class ProductCreateReqDto
{
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int TaxId { get; set; }
    public ProductPresentationCreateReqDto DefaultPresentation { get; set; } = new();
    public List<ProductPresentationCreateReqDto> Presentations { get; set; } = [];
}

public class ProductPresentationCreateReqDto
{
    public int UnitMeasureId { get; set; }
    public decimal Price01 { get; set; }
    public decimal Price02 { get; set; }
    public decimal Price03 { get; set; }
    public decimal Price04 { get; set; }
    public decimal NetWeight { get; set; }
    public decimal GrossWeight { get; set; }
    public bool IsActive { get; set; } = true;
}