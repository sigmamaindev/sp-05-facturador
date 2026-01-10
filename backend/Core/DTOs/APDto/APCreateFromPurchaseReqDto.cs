namespace Core.DTOs.APDto;

public class APCreateFromPurchaseReqDto
{
    public int TermDays { get; set; }
    public DateTime? ExpectedPaymentDate { get; set; }
    public decimal? InitialPaymentAmount { get; set; }
    public string? InitialPaymentMethodCode { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
}
