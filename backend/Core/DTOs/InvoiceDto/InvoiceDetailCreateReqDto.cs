using System;

namespace Core.DTOs.InvoiceDto;

public class InvoiceDetailCreateReqDto
{
    public int ProductId { get; set; }
    public int UnitMeasureId { get; set; }
    public int WarehouseId { get; set; }
    public int TaxId { get; set; }
    public decimal Quantity { get; set; }
     public decimal NetWeight { get; set; }
    public decimal GrossWeight { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
}
