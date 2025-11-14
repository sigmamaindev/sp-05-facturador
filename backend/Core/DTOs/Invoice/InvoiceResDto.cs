using Core.DTOs.Business;
using Core.DTOs.Customer;
using Core.DTOs.EmissionPoint;
using Core.DTOs.Establishment;
using Core.DTOs.User;

namespace Core.DTOs.Invoice;

public class InvoiceResDto
{
    public int Id { get; set; }
    public string Sequential { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string Environment { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsElectronic { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime? AuthorizationDate { get; set; }
    public CustomerResDto? Customer { get; set; }
    public BusinessResDto? Business { get; set; }
    public EstablishmentResDto? Establishment { get; set; }
    public EmissionPointResDto? EmissionPoint { get; set; }
    public UserResDto? User { get; set; }
    public decimal SubtotalWithoutTaxes { get; set; }
    public decimal SubtotalWithTaxes { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal TotalInvoice { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public int PaymentTermDays { get; set; }
    public DateTime DueDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? AdditionalInformation { get; set; }
    public string XmlSigned { get; set; } = string.Empty;
    public string? AuthorizationNumber { get; set; }
    public string? SriMessage { get; set; }
    public ICollection<InvoiceDetailResDto> InvoiceDetails { get; set; } = [];
}
