namespace Core.DTOs.ReportDto;

public class SalesReportResDto
{
    public DateTime InvoiceDate { get; set; }
    public string Sequential { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public int PaymentTermDays { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal GrossWeight { get; set; }
    public decimal NetWeight { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
}
