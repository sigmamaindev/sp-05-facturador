namespace Core.DTOs.ARDto;

public class ARCreateFromInvoiceReqDto
{
    public string PaymentMethod { get; set; } = string.Empty;
    public int TermDays { get; set; }
    public DateTime? ExpectedPaymentDate { get; set; }
    public decimal? InitialPaymentAmount { get; set; }
    public string? InitialPaymentMethodCode { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
}
