namespace Core.DTOs.Invoice;

public class InvoiceCreateReqDto
{
    public string Sequential { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal TotalInvoice { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int BusinessId { get; set; }
    public int CustomerId { get; set; }
    public int EstablishmentId { get; set; }
    public int EmissionPointId { get; set; }
    public List<InvoiceDetailsCreateReqDto> Details { get; set; } = [];
    public List<InvoiceTaxTotalReqDto> TaxTotals { get; set; } = [];
}
