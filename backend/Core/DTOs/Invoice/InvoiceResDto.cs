using Core.DTOs.Business;
using Core.DTOs.Customer;

namespace Core.DTOs.Invoice;

public class InvoiceResDto
{
    public int Id { get; set; }
    public string Sequential { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal TotalInvoice { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
}
