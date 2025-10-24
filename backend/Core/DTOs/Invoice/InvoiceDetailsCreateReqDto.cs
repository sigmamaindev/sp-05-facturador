using System;

namespace Core.DTOs.Invoice;

public class InvoiceDetailsCreateReqDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal Subtotal { get; set; }
    public decimal IvaValue { get; set; }
    public decimal Total { get; set; }
    public decimal TaxRate { get; set; }
    public int ProductId { get; set; }
    public int ProductTypeId { get; set; }
}
