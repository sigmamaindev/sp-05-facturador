using Core.Constants;

namespace Core.Entities;

public class AccountsReceivable : BaseEntity
{
    public int BusinessId { get; set; }
    public Business? Business { get; set; }
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int InvoiceId { get; set; }
    public Invoice? Invoice { get; set; }
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public DateTime? ExpectedPaymentDate { get; set; }
    public decimal OriginalAmount { get; set; }
    public decimal Balance { get; set; }
    public string Status { get; set; } = AccountStatus.OPEN;
    public ICollection<ARTransaction> Transactions { get; set; } = [];
}

