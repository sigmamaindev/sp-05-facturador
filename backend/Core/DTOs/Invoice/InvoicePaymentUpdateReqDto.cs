namespace Core.DTOs.Invoice;

public class InvoicePaymentUpdateReqDto
{
    public string PaymentMethod { get; set; } = string.Empty;
    public int PaymentTermDays { get; set; }
}
