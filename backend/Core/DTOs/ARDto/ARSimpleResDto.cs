namespace Core.DTOs.ARDto;

using Core.DTOs.CustomerDto;

public class ARSimpleResDto
{
    public int Id { get; set; }
    public ARInvoiceSimpleResDto? Invoice { get; set; }
    public CustomerResDto? Customer { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ExpectedPaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal OriginalAmount { get; set; }
    public decimal Balance { get; set; }
    public string Status { get; set; } = string.Empty;
}
