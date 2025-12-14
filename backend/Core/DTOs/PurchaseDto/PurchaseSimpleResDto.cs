namespace Core.DTOs.PurchaseDto;

public class PurchaseSimpleResDto
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public int EstablishmentId { get; set; }
    public int EmissionPointId { get; set; }
    public int SupplierId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public decimal SubtotalWithoutTaxes { get; set; }
    public decimal SubtotalWithTaxes { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal TotalPurchase { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<PurchaseDetailResDto> Details { get; set; } = [];
}
