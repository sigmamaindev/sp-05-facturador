namespace Core.DTOs.KardexDto;

public class KardexReportResDto
{
    public DateTime MovementDate { get; set; }
    public string MovementType { get; set; } = string.Empty;
    public string WarehouseCode { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;

    // ENTRADAS
    public decimal EntryQuantity { get; set; }
    public decimal EntryCost { get; set; }
    public decimal EntryTotal { get; set; }

    // SALIDAS
    public decimal ExitQuantity { get; set; }
    public decimal ExitCost { get; set; }
    public decimal ExitTotal { get; set; }

    // TOTALES (running)
    public decimal RunningStock { get; set; }
    public decimal RunningValue { get; set; }
}
