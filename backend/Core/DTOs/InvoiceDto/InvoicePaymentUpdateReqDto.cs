namespace Core.DTOs.InvoiceDto;

public class InvoicePaymentUpdateReqDto
{
    public string PaymentMethod { get; set; } = string.Empty;
    public int PaymentTermDays { get; set; }
}
