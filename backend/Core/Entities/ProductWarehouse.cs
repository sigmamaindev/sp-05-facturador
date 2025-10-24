namespace Core.Entities;

public class ProductWarehouse : BaseEntity
{
    public decimal Stock { get; set; } = 0;
    public decimal MinStock { get; set; } = 0;
    public decimal MaxStock { get; set; } = 0;
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }
}
