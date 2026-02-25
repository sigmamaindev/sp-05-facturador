namespace Core.DTOs.ReportDto;

public class PurchasesReportResDto
{
    public DateTime IssueDate { get; set; }
    public string Sequential { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal GrossWeight { get; set; }
    public decimal NetWeight { get; set; }
    public decimal UnitCost { get; set; }
    public decimal Total { get; set; }
}
