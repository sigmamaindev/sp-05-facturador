namespace Core.Entities;

public class Purchase : BaseEntity
{
    public int BusinessId { get; set; }
    public Business? Business { get; set; }
    public int EstablishmentId { get; set; }
    public Establishment? Establishment { get; set; }
    public int EmissionPointId { get; set; }
    public EmissionPoint? EmissionPoint { get; set; }
    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public decimal SubtotalWithoutTaxes { get; set; }
    public decimal SubtotalWithTaxes { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal TotalPurchase { get; set; }
    public string Status { get; set; } = string.Empty;
    public ICollection<PurchaseDetail> PurchaseDetails { get; set; } = [];
}
