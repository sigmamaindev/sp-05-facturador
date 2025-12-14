namespace Core.Entities;

public class PurchaseDetail : BaseEntity
{
    public int PurchaseId { get; set; }
    public Purchase? Purchase { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }
    public int TaxId { get; set; }
    public Tax? Tax { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxValue { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal Discount { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
}
