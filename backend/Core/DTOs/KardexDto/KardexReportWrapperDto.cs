namespace Core.DTOs.KardexDto;

public class KardexReportWrapperDto
{
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessAddress { get; set; } = string.Empty;
    public string BusinessRuc { get; set; } = string.Empty;

    public string ProductSku { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;

    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public DateTime ReportDate { get; set; }

    public List<KardexReportResDto> Movements { get; set; } = [];
}
