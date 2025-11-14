namespace Core.DTOs.Invoice;

public class InvoiceDetailResDto
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public int ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string UnitMeasure { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public int TaxId { get; set; }
    public string TaxCode { get; set; } = string.Empty;
    public string TaxName { get; set; } = string.Empty;
    public decimal TaxRate { get; set; }
    public decimal TaxValue { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
    public decimal TotalWithoutTax => Subtotal - Discount;
    public decimal TotalWithTax => Total;
    public string AdditionalDetail { get; set; } = string.Empty;
}
