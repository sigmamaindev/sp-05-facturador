namespace Core.DTOs.Invoice;

public class InvoiceCreateReqDto
{
    public string ReceiptType { get; set; } = string.Empty;
    public bool IsElectronic { get; set; }
    public string Environment { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public int CustomerId { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public int PaymentTermDays { get; set; }
    public string Description { get; set; } = string.Empty;
    public string AdditionalInformation { get; set; } = string.Empty;
    public List<InvoiceDetailCreateReqDto> Details { get; set; } = [];
}
