namespace Core.Entities;

public class Purchase : BaseEntity
{
    public int BusinessId { get; set; }
    public int EstablishmentId { get; set; }
    public int WarehouseId { get; set; }
    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal TotalTax { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public Warehouse? Warehouse { get; set; }
}
