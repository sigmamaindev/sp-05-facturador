namespace Core.Entities;

public class Kardex : BaseEntity
{
    public int BusinessId { get; set; }
    public Business? Business { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }
    public DateTime MovementDate { get; set; }
    public decimal QuantityIn { get; set; }
    public decimal QuantityOut { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public string MovementType { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
}
