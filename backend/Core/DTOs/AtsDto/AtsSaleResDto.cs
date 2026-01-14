namespace Core.DTOs.AtsDto;

public class AtsSaleResDto
{
    public string TpIdCliente { get; set; } = string.Empty;
    public string IdCliente { get; set; } = string.Empty;
    public string ParteRelVtas { get; set; } = "NO";
    public string TipoComprobante { get; set; } = string.Empty;
    public string TipoEmision { get; set; } = string.Empty;
    public int NumeroComprobantes { get; set; }
    public decimal BaseNoGraIva { get; set; }
    public decimal BaseImponible { get; set; }
    public decimal BaseImpGrav { get; set; }
    public decimal BaseImpExe { get; set; }
    public decimal MontoIce { get; set; }
    public decimal MontoIva { get; set; }
    public decimal Total { get; set; }
    public string ClienteRazonSocial { get; set; } = string.Empty;
}

