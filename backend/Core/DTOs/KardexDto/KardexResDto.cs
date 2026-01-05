namespace Core.DTOs.KardexDto;

public class KardexResDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductSku { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public DateTime MovementDate { get; set; }
    public string MovementType { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public decimal QuantityIn { get; set; }
    public decimal QuantityOut { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
}

