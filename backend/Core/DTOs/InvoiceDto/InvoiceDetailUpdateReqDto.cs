namespace Core.DTOs.InvoiceDto;

public class InvoiceDetailUpdateReqDto
{
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    public int TaxId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public string AdditionalDetail { get; set; } = string.Empty;
}
