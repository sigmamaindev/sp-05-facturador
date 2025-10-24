namespace Core.Entities;

public class InvoiceTaxTotal : BaseEntity
{
    public decimal TaxRate { get; set; }
    public decimal TaxableBase { get; set; }
    public decimal TaxValue { get; set; }
    public string TaxCode { get; set; } = "IVA";
    public int InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
}
