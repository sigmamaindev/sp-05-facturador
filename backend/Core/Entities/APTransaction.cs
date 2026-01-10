namespace Core.Entities;

public class APTransaction : BaseEntity
{
    public string APTransactionType { get; set; } = string.Empty;
    public int AccountsPayableId { get; set; }
    public AccountsPayable? AccountsPayable { get; set; }
    public decimal Amount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public string? PaymentDetails { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
