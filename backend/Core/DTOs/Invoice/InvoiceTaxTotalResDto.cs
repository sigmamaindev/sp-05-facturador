namespace Core.DTOs.Invoice;

public class InvoiceTaxTotalResDto
{
    public int Id { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxableBase { get; set; }
    public decimal TaxValue { get; set; }
    public string TaxCode { get; set; } = string.Empty;
}
