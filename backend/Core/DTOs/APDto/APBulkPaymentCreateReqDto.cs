namespace Core.DTOs.APDto;

public class APBulkPaymentCreateReqDto
{
    public string PaymentMethod { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<APBulkPaymentItemReqDto> Items { get; set; } = [];
}

public class APBulkPaymentItemReqDto
{
    public int AccountsPayableId { get; set; }
    public decimal Amount { get; set; }
}

