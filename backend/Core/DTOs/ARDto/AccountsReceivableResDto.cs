namespace Core.DTOs.ARDto;

public class AccountsReceivableResDto
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public int CustomerId { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ExpectedPaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal OriginalAmount { get; set; }
    public decimal Balance { get; set; }
    public string Status { get; set; } = string.Empty;
}
