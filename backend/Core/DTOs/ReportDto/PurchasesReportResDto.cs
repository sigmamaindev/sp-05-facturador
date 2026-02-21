namespace Core.DTOs.ReportDto;

public class PurchasesReportResDto
{
    public int Id { get; set; }
    public DateTime IssueDate { get; set; }
    public string Sequential { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string SupplierDocument { get; set; } = string.Empty;
    public decimal TotalPurchase { get; set; }
    public string Status { get; set; } = string.Empty;
}
