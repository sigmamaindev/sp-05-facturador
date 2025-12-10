namespace Core.DTOs.PurchaseDto;

public class PurchaseDetailCreateReqDto
{
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public int? TaxId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TaxRate { get; set; }
}

