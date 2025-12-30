using Core.Constants;

namespace Core.Entities;

public class Product : BaseEntity
{
    public required string Sku { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int BusinessId { get; set; }
    public Business? Business { get; set; }
    public int TaxId { get; set; }
    public Tax? Tax { get; set; }
    public string Type { get; set; } = ProductTypes.GOOD;
    public ICollection<ProductPresentation> ProductPresentations { get; set; } = [];
    public ICollection<ProductWarehouse> ProductWarehouses { get; set; } = [];
    public ICollection<InvoiceDetail> InvoiceDetails { get; set; } = [];
    public ICollection<PurchaseDetail> PurchaseDetails { get; set; } = [];
    public ICollection<Kardex> Kardexes { get; set; } = [];
}
