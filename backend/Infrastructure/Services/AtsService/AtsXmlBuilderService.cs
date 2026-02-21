using System.Globalization;
using System.Xml.Linq;
using Core.DTOs.AtsDto;
using Core.Entities;
using Core.Interfaces.Services.IAtsService;

namespace Infrastructure.Services.AtsService;

public class AtsXmlBuilderService : IAtsXmlBuilderService
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    public string BuildAtsXml(int year, int month, Business business, string numEstabRuc,
        IEnumerable<AtsPurchaseResDto> purchases, IEnumerable<AtsSaleResDto> sales,
        decimal totalVentas)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(year, 1900);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(month, 12);
        ArgumentOutOfRangeException.ThrowIfLessThan(month, 1);
        ArgumentNullException.ThrowIfNull(business);
        ArgumentNullException.ThrowIfNull(purchases);
        ArgumentNullException.ThrowIfNull(sales);

        var root = new XElement("iva",
            new XElement("TipoIDInformante", InferTipoIdInformante(business.Document)),
            new XElement("IdInformante", business.Document),
            new XElement("razonSocial", business.Name),
            new XElement("Anio", year),
            new XElement("Mes", month.ToString("D2", Culture)),
            new XElement("numEstabRuc", numEstabRuc),
            new XElement("totalVentas", FormatDecimal(totalVentas)),
            new XElement("codigoOperativo", "IVA"),
            BuildCompras(purchases),
            BuildVentas(sales),
            BuildVentasEstablecimiento(numEstabRuc, totalVentas));

        var document = new XDocument(new XDeclaration("1.0", "UTF-8", "no"), root);
        return document.ToString();
    }

    private static XElement BuildCompras(IEnumerable<AtsPurchaseResDto> purchases)
    {
        return new XElement("compras", purchases.Select(BuildCompra));
    }

    private static XElement BuildCompra(AtsPurchaseResDto purchase)
    {
        var elements = new List<XElement>
        {
            new("codSustento", purchase.CodSustento),
            new("tpIdProv", purchase.TpIdProv),
            new("idProv", purchase.IdProv),
            new("tipoComprobante", purchase.TipoComprobante),
            new("parteRel", purchase.ParteRel),
            new("fechaRegistro", FormatDate(purchase.FechaRegistro)),
            new("establecimiento", purchase.Establecimiento),
            new("puntoEmision", purchase.PuntoEmision),
            new("secuencial", purchase.Secuencial),
            new("fechaEmision", FormatDate(purchase.FechaEmision)),
            new("autorizacion", purchase.Autorizacion),
            new("baseNoGraIva", FormatDecimal(purchase.BaseNoGraIva)),
            new("baseImponible", FormatDecimal(purchase.BaseImponible)),
            new("baseImpGrav", FormatDecimal(purchase.BaseImpGrav)),
            new("baseImpExe", FormatDecimal(purchase.BaseImpExe)),
            new("montoIce", FormatDecimal(purchase.MontoIce)),
            new("montoIva", FormatDecimal(purchase.MontoIva)),
            new("valRetBien10", FormatDecimal(purchase.ValRetBien10)),
            new("valRetServ20", FormatDecimal(purchase.ValRetServ20)),
            new("valorRetBienes", FormatDecimal(purchase.ValorRetBienes)),
            new("valRetServ50", FormatDecimal(purchase.ValRetServ50)),
            new("valorRetServicios", FormatDecimal(purchase.ValorRetServicios)),
            new("valRetServ100", FormatDecimal(purchase.ValRetServ100)),
            new("totbasesImpReemb", FormatDecimal(purchase.TotbasesImpReemb)),
            new("pagoExterior",
                new XElement("pagoLocExt", purchase.PagoLocExt),
                new XElement("paisEfecPago", purchase.PaisEfecPago),
                new XElement("aplicConvDobTrib", purchase.AplicConvDobTrib),
                new XElement("pagExtSujRetNorLeg", purchase.PagExtSujRetNorLeg),
                new XElement("pagoRegFis", purchase.PagoRegFis))
        };

        if (!string.IsNullOrEmpty(purchase.FormaPago))
        {
            elements.Add(new XElement("formasDePago",
                new XElement("formaPago", purchase.FormaPago)));
        }

        if (purchase.AirDetails.Count > 0)
        {
            elements.Add(new XElement("air",
                purchase.AirDetails.Select(d => new XElement("detalleAir",
                    new XElement("codRetAir", d.CodRetAir),
                    new XElement("baseImpAir", FormatDecimal(d.BaseImpAir)),
                    new XElement("porcentajeAir", FormatDecimal(d.PorcentajeAir)),
                    new XElement("valRetAir", FormatDecimal(d.ValRetAir))))));
        }
        else
        {
            elements.Add(new XElement("air"));
        }

        if (!string.IsNullOrEmpty(purchase.EstabRetencion1))
        {
            elements.Add(new XElement("estabRetencion1", purchase.EstabRetencion1));
            elements.Add(new XElement("ptoEmiRetencion1", purchase.PtoEmiRetencion1));
            elements.Add(new XElement("secRetencion1", purchase.SecRetencion1));
            elements.Add(new XElement("autRetencion1", purchase.AutRetencion1));
            elements.Add(new XElement("fechaEmiRet1", purchase.FechaEmiRet1.HasValue
                ? FormatDate(purchase.FechaEmiRet1.Value)
                : string.Empty));
        }

        return new XElement("detalleCompras", elements);
    }

    private static XElement BuildVentas(IEnumerable<AtsSaleResDto> sales)
    {
        return new XElement("ventas", sales.Select(BuildVenta));
    }

    private static XElement BuildVenta(AtsSaleResDto sale)
    {
        var elements = new List<XElement>
        {
            new("tpIdCliente", sale.TpIdCliente),
            new("idCliente", sale.IdCliente),
            new("parteRelVtas", sale.ParteRelVtas),
            new("tipoComprobante", sale.TipoComprobante),
            new("tipoEmision", sale.TipoEmision),
            new("numeroComprobantes", sale.NumeroComprobantes),
            new("baseNoGraIva", FormatDecimal(sale.BaseNoGraIva)),
            new("baseImponible", FormatDecimal(sale.BaseImponible)),
            new("baseImpGrav", FormatDecimal(sale.BaseImpGrav)),
            new("montoIva", FormatDecimal(sale.MontoIva)),
            new("montoIce", FormatDecimal(sale.MontoIce)),
            new("valorRetIva", FormatDecimal(sale.ValorRetIva)),
            new("valorRetRenta", FormatDecimal(sale.ValorRetRenta))
        };

        if (!string.IsNullOrEmpty(sale.FormaPago))
        {
            elements.Add(new XElement("formasDePago",
                new XElement("formaPago", sale.FormaPago)));
        }

        return new XElement("detalleVentas", elements);
    }

    private static XElement BuildVentasEstablecimiento(string codEstab, decimal totalVentas)
    {
        return new XElement("ventasEstablecimiento",
            new XElement("ventaEst",
                new XElement("codEstab", codEstab),
                new XElement("ventasEstab", FormatDecimal(totalVentas))));
    }

    private static string InferTipoIdInformante(string document)
    {
        if (string.IsNullOrWhiteSpace(document))
        {
            return "R";
        }

        return document.Trim().Length switch
        {
            13 => "R",
            10 => "C",
            _ => "R"
        };
    }

    private static string FormatDate(DateTime date)
        => date.ToString("dd/MM/yyyy", Culture);

    private static string FormatDecimal(decimal value, int decimals = 2)
    {
        var format = "0." + new string('0', decimals);
        return decimal.Round(value, decimals, MidpointRounding.AwayFromZero).ToString(format, Culture);
    }
}
