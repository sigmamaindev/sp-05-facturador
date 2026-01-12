namespace Core.DTOs.ARDto;

public class ARBulkPaymentCreateReqDto
{
    public string PaymentMethod { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<ARBulkPaymentItemReqDto> Items { get; set; } = [];
}

public class ARBulkPaymentItemReqDto
{
    public int AccountsReceivableId { get; set; }
    public decimal Amount { get; set; }
}
