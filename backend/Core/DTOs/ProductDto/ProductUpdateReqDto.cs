using Core.Constants;
using Core.DTOs.InventoryDto;

namespace Core.DTOs.ProductDto;

public class ProductUpdateReqDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = ProductTypes.GOOD;
    public int TaxId { get; set; }
    public ProductPresentationUpdateReqDto DefaultPresentation { get; set; } = new();
    public List<ProductPresentationUpdateReqDto> Presentations { get; set; } = [];
    public List<InventoryUpdateReqDto> Inventory { get; set; } = [];
}

public class ProductPresentationUpdateReqDto
{
    public int? Id { get; set; }
    public int UnitMeasureId { get; set; }
    public decimal Price01 { get; set; }
    public decimal Price02 { get; set; }
    public decimal Price03 { get; set; }
    public decimal Price04 { get; set; }
    public decimal NetWeight { get; set; }
    public decimal GrossWeight { get; set; }
    public bool IsDefault { get; set; } = false;
    public bool IsActive { get; set; } = true;
}
