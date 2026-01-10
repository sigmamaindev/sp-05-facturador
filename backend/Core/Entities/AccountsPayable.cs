using Core.Constants;

namespace Core.Entities;

public class AccountsPayable : BaseEntity
{
    public int BusinessId { get; set; }
    public Business? Business { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public int PurchaseId { get; set; }
    public Purchase? Purchase { get; set; }
    public required string DocumentNumber { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal Total { get; set; }
    public decimal Balance { get; set; }
    public string Status { get; set; } = AccountStatus.OPEN;
    public string? Notes { get; set; }
    public DateTime IssueDate { get; set; } = DateTime.UtcNow;
    public DateTime DueDate { get; set; }
    public DateTime? ExpectedPaymentDate { get; set; }
    public ICollection<APTransaction> Transactions { get; set; } = [];
}
