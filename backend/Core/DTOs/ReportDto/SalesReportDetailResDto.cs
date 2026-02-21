namespace Core.DTOs.ReportDto;

public class SalesReportDetailResDto
{
    public int Id { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string Sequential { get; set; } = string.Empty;
    public int PaymentTermDays { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerDocument { get; set; } = string.Empty;
    public decimal SubtotalWithoutTaxes { get; set; }
    public decimal SubtotalWithTaxes { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal TotalInvoice { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<SalesReportDetailItemResDto> Items { get; set; } = [];
}

public class SalesReportDetailItemResDto
{
    public int Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string UnitMeasureName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxValue { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
}
