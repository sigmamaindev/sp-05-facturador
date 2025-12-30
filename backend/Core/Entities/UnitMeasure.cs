namespace Core.Entities;

public class UnitMeasure : BaseEntity
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public decimal FactorBase { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public int BusinessId { get; set; }
    public Business? Business { get; set; }
    public ICollection<Product> Products { get; set; } = [];
    public ICollection<ProductPresentation> ProductPresentations { get; set; } = [];
    public ICollection<InvoiceDetail> InvoiceDetails { get; set; } = [];
}
