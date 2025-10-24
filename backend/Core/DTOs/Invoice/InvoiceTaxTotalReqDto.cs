namespace Core.DTOs.Invoice;

public class InvoiceTaxTotalReqDto
{
    public string TaxCode { get; set; } = "IVA";
    public decimal TaxRate { get; set; }
    public decimal TaxableBase { get; set; }
    public decimal TaxValue { get; set; }
}
