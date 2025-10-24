using Core.Constants;

namespace Core.Entities;

public class Invoice : BaseEntity
{
    public required string Sequential { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal TotalInvoice { get; set; }
    public string Status { get; set; } = InvoiceStatuses.Draft;
    public string? Description { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int BusinessId { get; set; }
    public Business? Business { get; set; }
    public int EstablishmentId { get; set; }
    public Establishment? Establishment { get; set; }
    public int EmissionPointId { get; set; }
    public EmissionPoint? EmissionPoint { get; set; }
    public ICollection<InvoiceDetails> InvoiceDetails = [];
    public ICollection<InvoiceTaxTotal> InvoiceTaxTotals = [];
}
