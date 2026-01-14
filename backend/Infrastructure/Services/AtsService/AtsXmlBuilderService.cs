using System.Globalization;
using System.Xml.Linq;
using Core.DTOs.AtsDto;
using Core.Entities;
using Core.Interfaces.Services.IAtsService;

namespace Infrastructure.Services.AtsService;

public class AtsXmlBuilderService : IAtsXmlBuilderService
{
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    public string BuildAtsPurchasesXml(int year, int month, Business business, IEnumerable<AtsPurchaseResDto> purchases)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(year, 1900);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(month, 12);
        ArgumentOutOfRangeException.ThrowIfLessThan(month, 1);
        ArgumentNullException.ThrowIfNull(business);
        ArgumentNullException.ThrowIfNull(purchases);

        var root = new XElement("iva",
            new XElement("TipoIDInformante", InferTipoIdInformante(business.Document)),
            new XElement("IdInformante", business.Document),
            new XElement("razonSocial", business.Name),
            new XElement("Anio", year),
            new XElement("Mes", month.ToString("D2", Culture)),
            new XElement("compras", purchases.Select(BuildCompra)));

        var document = new XDocument(new XDeclaration("1.0", "UTF-8", null), root);
        return document.ToString();
    }

    private static XElement BuildCompra(AtsPurchaseResDto purchase)
    {
        return new XElement("detalleCompras",
            new XElement("codSustento", purchase.CodSustento),
            new XElement("tpIdProv", purchase.TpIdProv),
            new XElement("idProv", purchase.IdProv),
            new XElement("tipoComprobante", purchase.TipoComprobante),
            new XElement("parteRel", purchase.ParteRel),
            new XElement("fechaRegistro", FormatDate(purchase.FechaRegistro)),
            new XElement("establecimiento", purchase.Establecimiento),
            new XElement("puntoEmision", purchase.PuntoEmision),
            new XElement("secuencial", purchase.Secuencial),
            new XElement("fechaEmision", FormatDate(purchase.FechaEmision)),
            new XElement("autorizacion", purchase.Autorizacion),
            new XElement("baseNoGraIva", FormatDecimal(purchase.BaseNoGraIva)),
            new XElement("baseImponible", FormatDecimal(purchase.BaseImponible)),
            new XElement("baseImpGrav", FormatDecimal(purchase.BaseImpGrav)),
            new XElement("baseImpExe", FormatDecimal(purchase.BaseImpExe)),
            new XElement("montoIce", FormatDecimal(purchase.MontoIce)),
            new XElement("montoIva", FormatDecimal(purchase.MontoIva)));
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

