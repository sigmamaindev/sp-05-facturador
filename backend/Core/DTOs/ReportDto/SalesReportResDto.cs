namespace Core.DTOs.ReportDto;

public class SalesReportResDto
{
    public int Id { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string Sequential { get; set; } = string.Empty;
    public int PaymentTermDays { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerDocument { get; set; } = string.Empty;
    public decimal TotalInvoice { get; set; }
    public string Status { get; set; } = string.Empty;
}
