namespace Core.DTOs.ARDto;

public class ARComplexResDto
{
    public int Id { get; set; }
    public string EstablishmentCode { get; set; } = string.Empty;
    public string EmissionPointCode { get; set; } = string.Empty;
    public string Sequential { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string? AuthorizationNumber { get; set; }
    public string Environment { get; set; } = string.Empty;
    public string ReceiptType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsElectronic { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime? AuthorizationDate { get; set; }
    public decimal OriginalAmount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public int PaymentTermDays { get; set; }
    public DateTime? DueDate { get; set; }
    public List<ARTransactionResDto> Transactions { get; set; } = [];
}
