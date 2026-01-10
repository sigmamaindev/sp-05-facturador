using Core.DTOs.SupplierDto;

namespace Core.DTOs.APDto;

public class APComplexResDto
{
    public int Id { get; set; }
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ExpectedPaymentDate { get; set; }
    public decimal Total { get; set; }
    public decimal Balance { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public APPurchaseResDto Purchase { get; set; } = new();
    public SupplierResDto Supplier { get; set; } = new();
    public List<APTransactionResDto> Transactions { get; set; } = [];
}
