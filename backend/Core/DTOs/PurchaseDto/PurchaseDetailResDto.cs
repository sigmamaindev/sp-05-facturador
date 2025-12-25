namespace Core.DTOs.PurchaseDto;

public class PurchaseDetailResDto
{
    public int Id { get; set; }
    public int PurchaseId { get; set; }
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public int TaxId { get; set; }
    public decimal Quantity { get; set; }
    public decimal NetWeight { get; set; }
    public decimal GrossWeight { get; set; }
    public decimal UnitCost { get; set; }
    public decimal Discount { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxValue { get; set; }
    public decimal Total { get; set; }
}
