using Core.Constants;

namespace Core.Entities;

public class Invoice : BaseEntity
{
    public required string Sequential { get; set; }
    public string AccessKey { get; set; } = string.Empty;
    public string Environment { get; set; } =string.Empty;
    public string ReceiptType { get; set; } = ReceiptCodeType.INVOICE;
    public string Status { get; set; } = InvoiceStatus.DRAFT;
    public bool IsElectronic { get; set; } = true;
    public DateTime InvoiceDate { get; set; }
    public DateTime? AuthorizationDate { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int BusinessId { get; set; }
    public Business? Business { get; set; }
    public int EstablishmentId { get; set; }
    public Establishment? Establishment { get; set; }
    public int EmissionPointId { get; set; }
    public EmissionPoint? EmissionPoint { get; set; }
    public decimal SubtotalWithoutTaxes { get; set; }
    public decimal SubtotalWithTaxes { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal TotalInvoice { get; set; }
    public string PaymentMethod { get; set; } = PaymentMethods.CASH;
    public int PaymentTermDays { get; set; } = 0;
    public DateTime? DueDate { get; set; }
    public string? Description { get; set; }
    public string? AdditionalInformation { get; set; }
    public string? XmlSigned { get; set; }
    public string? AuthorizationNumber { get; set; }
    public string? SriMessage { get; set; }
    public ICollection<InvoiceDetail> InvoiceDetails { get; set; } = [];
}
