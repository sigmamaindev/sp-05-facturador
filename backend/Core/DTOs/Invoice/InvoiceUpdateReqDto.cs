namespace Core.DTOs.Invoice;

public class InvoiceUpdateReqDto
{
    public string DocumentType { get; set; } = string.Empty;
    public bool IsElectronic { get; set; }
    public string Environment { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal SubtotalWithoutTaxes { get; set; }
    public decimal SubtotalWithTaxes { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal TotalInvoice { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public int PaymentTermDays { get; set; }
    public string Description { get; set; } = string.Empty;
    public string AdditionalInformation { get; set; } = string.Empty;
}
