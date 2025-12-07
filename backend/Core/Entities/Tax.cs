using Core.Constants;

namespace Core.Entities;

public class Tax : BaseEntity
{
    public required string Code { get; set; }
    public required string CodePercentage { get; set; }
    public required string Name { get; set; }
    public string Group { get; set; } = TaxCode.IVA;
    public decimal Rate { get; set; }
    public bool IsActive { get; set; } = true;
    public int BusinessId { get; set; }
    public Business? Business { get; set; }
    public ICollection<Product> Products { get; set; } = [];
    public ICollection<InvoiceDetail> InvoiceDetails { get; set; } = [];
}
