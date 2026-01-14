using Core.DTOs.SupplierDto;

namespace Core.DTOs.APDto;

public class APSupplierSummaryResDto
{
    public SupplierResDto Supplier { get; set; } = new();
    public decimal TotalBalance { get; set; }
}

