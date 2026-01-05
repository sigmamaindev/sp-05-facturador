namespace Core.DTOs.PurchaseDto;

public class PurchaseDetailResDto
{
    public int Id { get; set; }
    public int PurchaseId { get; set; }
    public int ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public int UnitMeasureId { get; set; }
    public string UnitMeasureCode { get; set; } = string.Empty;
    public string UnitMeasureName { get; set; } = string.Empty;
    public int TaxId { get; set; }
    public string TaxCode { get; set; } = string.Empty;
    public string TaxName { get; set; } = string.Empty;
    public decimal TaxRate { get; set; }
    public decimal TaxValue { get; set; }
    public decimal Quantity { get; set; }
    public decimal NetWeight { get; set; }
    public decimal GrossWeight { get; set; }
    public decimal UnitCost { get; set; }
    public decimal Discount { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
}
