using System.Globalization;
using System.Xml.Linq;
using Core.Constants;
using Core.Entities;
using Core.Interfaces.Services;

namespace Infrastructure.Services;

public class InvoiceXmlBuilder : IInvoiceXmlBuilder
{
    private const string InvoiceVersion = "1.1.0";
    private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    public string BuildXMLInvoice(Invoice invoice, Business business, Establishment establishment, EmissionPoint emissionPoint, Customer customer)
    {
        ArgumentNullException.ThrowIfNull(invoice);
        ArgumentNullException.ThrowIfNull(business);
        ArgumentNullException.ThrowIfNull(establishment);
        ArgumentNullException.ThrowIfNull(emissionPoint);
        ArgumentNullException.ThrowIfNull(customer);

        if (invoice.InvoiceDetails == null || invoice.InvoiceDetails.Count == 0)
        {
            throw new InvalidOperationException("La factura no contiene detalles para generar el XML del SRI.");
        }

        var invoiceContent = new List<object>
        {
            TaxInfoBuilder(invoice, business, establishment, emissionPoint),
            InvoiceInfoBuilder(invoice, business, customer),
            InvoiceDetailsBuilder(invoice)
        };

        var addcionalInfo = AddicionalInfoDetail(invoice, customer);

        if (addcionalInfo != null)
        {
            invoiceContent.Add(addcionalInfo);
        }

        var facturaElement = new XElement("factura",
            new XAttribute("id", "comprobante"),
            new XAttribute("version", InvoiceVersion),
            invoiceContent);

        var document = new XDocument(new XDeclaration("1.0", "UTF-8", null), facturaElement);

        return document.ToString();
    }

    private static XElement TaxInfoBuilder(Invoice invoice, Business business, Establishment establishment, EmissionPoint emissionPoint)
    {
        return new XElement("infoTributaria",
            new XElement("ambiente", invoice.Environment),
            new XElement("tipoEmision", EmissionTypes.NORMAL),
            new XElement("razonSocial", business.Name),
            new XElement("nombreComercial", establishment.Name),
            new XElement("ruc", business.Document),
            new XElement("claveAcceso", invoice.AccessKey),
            new XElement("codDoc", invoice.DocumentType),
            new XElement("estab", establishment.Code),
            new XElement("ptoEmi", emissionPoint.Code),
            new XElement("secuencial", invoice.Sequential),
            new XElement("dirMatriz", business.Address)
        );
    }

    private static XElement InvoiceInfoBuilder(Invoice invoice, Business business, Customer customer)
    {
        var emissionDate = invoice.InvoiceDate == default
            ? DateTime.UtcNow
            : invoice.InvoiceDate;

        return new XElement("infoFactura",
            new XElement("fechaEmision", emissionDate.ToString("dd/MM/yyyy", Culture)),
            new XElement("dirEstablecimiento", business.Address),
            new XElement("obligadoContabilidad", "NO"),
            new XElement("tipoIdentificacionComprador", customer.DocumentType),
            new XElement("razonSocialComprador", customer.Name),
            new XElement("identificacionComprador", customer.Document),
            new XElement("direccionComprador", customer.Address),
            new XElement("totalSinImpuestos", FormatDecimal(invoice.SubtotalWithoutTaxes)),
            new XElement("totalDescuento", FormatDecimal(invoice.DiscountTotal)),
            BuildTotalWithTaxes(invoice),
            new XElement("propina", FormatDecimal(0m)),
            new XElement("importeTotal", FormatDecimal(invoice.TotalInvoice)),
            new XElement("moneda", Currencies.USD),
            BuildPayments(invoice)
        );
    }

    private static XElement BuildTotalWithTaxes(Invoice invoice)
    {
        var groupedTaxes = invoice.InvoiceDetails
            .Where(d => d.Subtotal > 0)
            .GroupBy(d => new
            {
                Code = d.Tax!.Code,
                CodePercentage = d.Tax.CodePercentage,
                Rate = d.Tax.Rate
            })
            .Select(group => new XElement("totalImpuesto",
                new XElement("codigo", group.Key.Code),
                new XElement("codigoPorcentaje", group.Key.CodePercentage),
                new XElement("baseImponible", FormatDecimal(group.Sum(d => d.Subtotal))),
                new XElement("tarifa", FormatDecimal(group.Key.Rate)),
                new XElement("valor", FormatDecimal(group.Sum(d => d.TaxValue)))
            ))
            .ToList();

        return new XElement("totalConImpuestos", groupedTaxes);
    }

    private static XElement BuildPayments(Invoice invoice)
    {
        var pago = new XElement("pago",
            new XElement("formaPago", string.IsNullOrWhiteSpace(invoice.PaymentMethod) ? PaymentMethods.CASH : invoice.PaymentMethod),
            new XElement("total", FormatDecimal(invoice.TotalInvoice))
        );

        if (invoice.PaymentTermDays > 0)
        {
            pago.Add(new XElement("plazo", invoice.PaymentTermDays));
            pago.Add(new XElement("unidadTiempo", "dias"));
        }

        return new XElement("pagos", pago);
    }

    private static XElement InvoiceDetailsBuilder(Invoice invoice)
    {
        var detailElements = new List<XElement>();

        foreach (var detail in invoice.InvoiceDetails)
        {
            var detailElement = new XElement("detalle",
                new XElement("codigoPrincipal", detail.Product?.Sku ?? detail.ProductId.ToString()),
                new XElement("descripcion", detail.Product?.Name ?? "Producto"),
                new XElement("cantidad", FormatDecimal(detail.Quantity, 6)),
                new XElement("precioUnitario", FormatDecimal(detail.UnitPrice)),
                new XElement("descuento", FormatDecimal(detail.Discount)),
                new XElement("precioTotalSinImpuesto", FormatDecimal(detail.Subtotal))
            );

            var additionalDetails = AdditionalDetailsBuilder(detail);
            if (additionalDetails != null)
            {
                detailElement.Add(additionalDetails);
            }

            detailElement.Add(BuildTaxesDetail(detail));

            detailElements.Add(detailElement);
        }

        return new XElement("detalles", detailElements);
    }

    private static XElement BuildTaxesDetail(InvoiceDetail detail)
    {
        var tax = detail.Tax ?? throw new Exception("El producto no tiene impuesto configurado.");

        var taxDetail = new XElement("impuesto",
        new XElement("codigo", tax.Code),
        new XElement("codigoPorcentaje", tax.CodePercentage),
        new XElement("tarifa", FormatDecimal(tax.Rate)),
        new XElement("baseImponible", FormatDecimal(detail.Subtotal)),
        new XElement("valor", FormatDecimal(detail.TaxValue))
        );

        return new XElement("impuestos", taxDetail);
    }

    private static XElement? AdditionalDetailsBuilder(InvoiceDetail detail)
    {
        var add = new List<XElement>();

        if (!string.IsNullOrWhiteSpace(detail.Product?.Description))
        {
            add.Add(new XElement("detAdicional",
                new XAttribute("nombre", "Detalle"),
                new XAttribute("valor", detail.Product.Description)
            ));
        }

        if (!string.IsNullOrWhiteSpace(detail.Warehouse?.Name))
        {
            add.Add(new XElement("detAdicional",
                new XAttribute("nombre", "Bodega"),
                new XAttribute("valor", detail.Warehouse.Name)
            ));
        }

        if (add.Count == 0)
            return null;

        return new XElement("detallesAdicionales", add);
    }

    private static XElement? AddicionalInfoDetail(Invoice invoice, Customer customer)
    {
        var campos = new List<XElement>();

        if (!string.IsNullOrWhiteSpace(customer.Email))
        {
            campos.Add(new XElement("campoAdicional",
                new XAttribute("nombre", "Email"),
                customer.Email));
        }

        if (!string.IsNullOrWhiteSpace(customer.Cellphone))
        {
            campos.Add(new XElement("campoAdicional",
                new XAttribute("nombre", "Celular"),
                customer.Cellphone));
        }

        if (!string.IsNullOrWhiteSpace(customer.Telephone))
        {
            campos.Add(new XElement("campoAdicional",
                new XAttribute("nombre", "Telefono"),
                customer.Telephone));
        }

        if (!string.IsNullOrWhiteSpace(invoice.AdditionalInformation))
        {
            campos.AddRange(ParseAdditionalInformation(invoice.AdditionalInformation));
        }

        if (campos.Count == 0)
        {
            return null;
        }

        return new XElement("infoAdicional", campos);
    }

    private static IEnumerable<XElement> ParseAdditionalInformation(string additionalInformation)
    {
        var separators = new char[] { ';', '\n', '\r' };
        var lines = additionalInformation.Split(separators, StringSplitOptions.RemoveEmptyEntries);


        if (lines.Length == 0)
        {
            yield return new XElement("campoAdicional",
                new XAttribute("nombre", "Informacion"),
                additionalInformation.Trim());
            yield break;
        }

        var unnamedIndex = 1;

        foreach (var rawLine in lines)
        {
            var line = rawLine.Trim();

            if (line.Length == 0)
            {
                continue;
            }

            var parts = line.Split(':', 2, StringSplitOptions.TrimEntries);

            if (parts.Length == 2)
            {
                yield return new XElement("campoAdicional",
                    new XAttribute("nombre", parts[0]),
                    parts[1]);
            }
            else
            {
                yield return new XElement("campoAdicional",
                    new XAttribute("nombre", $"Adicional{unnamedIndex++}"),
                    line);
            }
        }
    }

    private static string FormatDecimal(decimal value, int decimals = 2)
    {
        var format = "0." + new string('0', decimals);
        return decimal.Round(value, decimals, MidpointRounding.AwayFromZero).ToString(format, Culture);
    }
}
