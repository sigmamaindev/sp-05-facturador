namespace Core.DTOs.InvoiceDto;

public class InvoicePaymentUpdateReqDto
{
    public string PaymentMethod { get; set; } = string.Empty;
    public int PaymentTermDays { get; set; }
    public DateTime ExpectedPaymentDate { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
