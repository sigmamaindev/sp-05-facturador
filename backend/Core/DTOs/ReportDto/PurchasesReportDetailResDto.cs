namespace Core.DTOs.ReportDto;

public class PurchasesReportDetailResDto
{
    public int Id { get; set; }
    public DateTime IssueDate { get; set; }
    public string Sequential { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string SupplierDocument { get; set; } = string.Empty;
    public decimal SubtotalWithoutTaxes { get; set; }
    public decimal SubtotalWithTaxes { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal TotalPurchase { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<PurchasesReportDetailItemResDto> Items { get; set; } = [];
}

public class PurchasesReportDetailItemResDto
{
    public int Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string UnitMeasureName { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxValue { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
}
