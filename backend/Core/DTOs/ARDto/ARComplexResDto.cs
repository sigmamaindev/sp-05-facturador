using Core.DTOs.CustomerDto;

namespace Core.DTOs.ARDto;

public class ARComplexResDto
{
    public int Id { get; set; }
    public ARInvoiceResDto? Invoice { get; set; }
    public CustomerResDto? Customer { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ExpectedPaymentDate { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public decimal OriginalAmount { get; set; }
    public decimal Balance { get; set; }
    public string Status { get; set; } = string.Empty;
     public List<ARTransactionResDto> Transactions { get; set; } = [];
}
