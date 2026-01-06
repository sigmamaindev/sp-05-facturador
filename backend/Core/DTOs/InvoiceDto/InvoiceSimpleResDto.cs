using Core.DTOs.CustomerDto;

namespace Core.DTOs.InvoiceDto;

public class InvoiceSimpleResDto
{
    public int Id { get; set; }
    public string Sequential { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string ReceiptType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsElectronic { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime? AuthorizationDate { get; set; }
    public CustomerResDto? Customer { get; set; }
    public int BusinessId { get; set; }
    public string BusinessDocument { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public int EstablishmentId { get; set; }
    public string EstablishmentCode { get; set; } = string.Empty;
    public string EstablishmentName { get; set; } = string.Empty;
    public int EmissionPointId { get; set; }
    public string EmissionPointCode { get; set; } = string.Empty;
    public string EmissionPointDescription { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserDocument { get; set; } = string.Empty;
    public string UserFullName { get; set; } = string.Empty;
    public decimal SubtotalWithoutTaxes { get; set; }
    public decimal SubtotalWithTaxes { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal TotalInvoice { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string? PaymentType { get; set; }
    public int PaymentTermDays { get; set; }
    public DateTime? DueDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? AdditionalInformation { get; set; }
    public string XmlSigned { get; set; } = string.Empty;
    public string? AuthorizationNumber { get; set; }
    public string? SriMessage { get; set; }
}
