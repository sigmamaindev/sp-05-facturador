namespace Core.DTOs.PurchaseDto;

public class PurchaseDetailCreateReqDto
{
    public int PurchaseId { get; set; }
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public int UnitMeasureId { get; set; }
    public int TaxId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal Discount { get; set; }
    public string AdditionalDetail { get; set; } = string.Empty;
}
