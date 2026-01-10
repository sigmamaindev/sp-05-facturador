namespace Core.DTOs.APDto;

public class APPurchaseResDto
{
    public int Id { get; set; }
    public string Sequential { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string ReceiptType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsElectronic { get; set; }
    public DateTime IssueDate { get; set; }
    public decimal TotalPurchase { get; set; }
}
