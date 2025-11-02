using Core.Constants;

namespace Core.Entities;

public class Product : BaseEntity
{
    public required string Sku { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public bool Iva { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public int BusinessId { get; set; }
    public Business? Business { get; set; }
    public int UnitMeasureId { get; set; }
    public UnitMeasure? UnitMeasure { get; set; }
    public int TaxId { get; set; }
    public Tax? Tax { get; set; }
    public string Type { get; set; } = ProductTypes.Good;
    public ICollection<ProductWarehouse> ProductWarehouses { get; set; } = [];
}
