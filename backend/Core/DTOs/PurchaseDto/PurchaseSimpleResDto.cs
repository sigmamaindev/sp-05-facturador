namespace Core.DTOs.PurchaseDto;

public class PurchaseSimpleResDto
{
    public int Id { get; set; }
    public int BusinessId { get; set; }
    public int EstablishmentId { get; set; }
    public int WarehouseId { get; set; }
    public int SupplierId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal TotalTax { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
}

