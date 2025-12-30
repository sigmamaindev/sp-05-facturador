using Core.DTOs.UnitMeasureDto;
using Core.DTOs.TaxDto;
using Core.DTOs.InventoryDto;

namespace Core.DTOs.ProductDto;

public class ProductResDto
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int TaxId { get; set; }
    public TaxResDto? Tax { get; set; }
    public bool IsActive { get; set; }
    public ProductPresentationResDto? DefaultPresentation { get; set; }
    public List<ProductPresentationResDto> Presentations { get; set; } = [];
    public List<InventoryResDto> Inventory { get; set; } = [];
}

public class ProductPresentationResDto
{
    public int Id { get; set; }
    public int UnitMeasureId { get; set; }
    public UnitMeasureResDto? UnitMeasure { get; set; }
    public decimal Price01 { get; set; }
    public decimal Price02 { get; set; }
    public decimal Price03 { get; set; }
    public decimal Price04 { get; set; }
    public decimal NetWeight { get; set; }
    public decimal GrossWeight { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}
