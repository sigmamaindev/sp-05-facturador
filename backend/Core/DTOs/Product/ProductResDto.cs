using Core.DTOs.UnitMeasure;
using Core.DTOs.Tax;
using Core.DTOs.Inventory;

namespace Core.DTOs.Product;

public class ProductResDto
{
    public int Id { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool Iva { get; set; }
    public bool IsActive { get; set; }
    public UnitMeasureResDto? UnitMeasure { get; set; }
    public TaxResDto? Tax { get; set; }
    public List<InventoryResDto> Inventory { get; set; } = [];
}
