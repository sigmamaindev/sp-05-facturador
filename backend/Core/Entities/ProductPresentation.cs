namespace Core.Entities;

public class ProductPresentation : BaseEntity
{
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int UnitMeasureId { get; set; }
    public UnitMeasure? UnitMeasure { get; set; }
    public decimal Price01 { get; set; }
    public decimal Price02 { get; set; }
    public decimal Price03 { get; set; }
    public decimal Price04 { get; set; }
    public decimal NetWeight { get; set; }
    public decimal GrossWeight { get; set; }
    public bool IsDefault { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public ICollection<InvoiceDetail> InvoiceDetails { get; set; } = [];
}
