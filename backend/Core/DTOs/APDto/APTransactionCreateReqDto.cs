namespace Core.DTOs.APDto;

public class APTransactionCreateReqDto
{
    public string APTransactionType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public string? PaymentDetails { get; set; }
}

