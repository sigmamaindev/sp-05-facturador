namespace Core.DTOs.ARDto;

public class ARTransactionResDto
{
    public int Id { get; set; }
    public string ARTransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public string? PaymentDetails { get; set; }
    public int AccountReceivableId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
