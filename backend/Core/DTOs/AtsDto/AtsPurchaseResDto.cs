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
    public decimal ValRetBien10 { get; set; }
    public decimal ValRetServ20 { get; set; }
    public decimal ValorRetBienes { get; set; }
    public decimal ValRetServ50 { get; set; }
    public decimal ValorRetServicios { get; set; }
    public decimal ValRetServ100 { get; set; }
    public decimal TotbasesImpReemb { get; set; }
    public string PagoLocExt { get; set; } = "01";
    public string PaisEfecPago { get; set; } = "NA";
    public string AplicConvDobTrib { get; set; } = "NA";
    public string PagExtSujRetNorLeg { get; set; } = "NA";
    public string PagoRegFis { get; set; } = "NA";
    public string? FormaPago { get; set; }
    public List<AtsAirDetailDto> AirDetails { get; set; } = [];
    public string? EstabRetencion1 { get; set; }
    public string? PtoEmiRetencion1 { get; set; }
    public string? SecRetencion1 { get; set; }
    public string? AutRetencion1 { get; set; }
    public DateTime? FechaEmiRet1 { get; set; }
    public decimal Total { get; set; }
    public string ProveedorRazonSocial { get; set; } = string.Empty;
}

public class AtsAirDetailDto
{
    public string CodRetAir { get; set; } = string.Empty;
    public decimal BaseImpAir { get; set; }
    public decimal PorcentajeAir { get; set; }
    public decimal ValRetAir { get; set; }
}
