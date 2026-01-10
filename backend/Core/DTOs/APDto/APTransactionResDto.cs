namespace Core.DTOs.APDto;

public class APTransactionResDto
{
    public int Id { get; set; }
    public string APTransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public string? PaymentDetails { get; set; }
    public int AccountsPayableId { get; set; }
    public DateTime CreatedAt { get; set; }
}
