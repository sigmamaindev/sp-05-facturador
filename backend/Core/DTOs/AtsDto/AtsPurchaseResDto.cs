namespace Core.DTOs.AtsDto;

public class AtsPurchaseResDto
{
    public int PurchaseId { get; set; }
    public string CodSustento { get; set; } = string.Empty;
    public string TpIdProv { get; set; } = string.Empty;
    public string IdProv { get; set; } = string.Empty;
    public string TipoComprobante { get; set; } = string.Empty;
    public string ParteRel { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }
    public string Establecimiento { get; set; } = string.Empty;
    public string PuntoEmision { get; set; } = string.Empty;
    public string Secuencial { get; set; } = string.Empty;
    public DateTime FechaEmision { get; set; }
    public string Autorizacion { get; set; } = string.Empty;
    public decimal BaseNoGraIva { get; set; }
    public decimal BaseImponible { get; set; }
    public decimal BaseImpGrav { get; set; }
    public decimal BaseImpExe { get; set; }
    public decimal MontoIce { get; set; }
    public decimal MontoIva { get; set; }
    public decimal Total { get; set; }
    public string ProveedorRazonSocial { get; set; } = string.Empty;
}

