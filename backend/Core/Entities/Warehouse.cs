namespace Core.Entities;

public class Warehouse : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool IsMain { get; set; } = false;
    public int BusinessId { get; set; }
    public Business? Business { get; set; }
    public ICollection<ProductWarehouse> ProductWarehouses { get; set; } = [];
    public ICollection<InvoiceDetail> InvoiceDetails { get; set; } = [];
    public ICollection<PurchaseDetail> PurchaseDetails { get; set; } = [];
    public ICollection<Kardex> Kardexes { get; set; } = [];
}
