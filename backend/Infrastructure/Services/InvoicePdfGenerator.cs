using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Core.Interfaces.Services;
using Core.DTOs.Invoice;

namespace Infrastructure.Services;

public class InvoicePdfGenerator : IInvoicePdfGenerator
{
    public byte[] Generate(InvoiceComplexResDto invoice)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var culture = new CultureInfo("es-EC");

        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(header =>
                {
                    header.Item().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(invoice.BusinessName).FontSize(18).SemiBold();
                            col.Item().Text($"RUC: {invoice.BusinessDocument}");
                            col.Item().Text($"Establecimiento: {invoice.EstablishmentCode} - {invoice.EstablishmentName}");
                            col.Item().Text($"Punto de Emisión: {invoice.EmissionPointCode} - {invoice.EmissionPointDescription}");
                        });

                        row.ConstantItem(220).Element(box =>
                        {
                            box.Border(1).Padding(10).Column(col =>
                            {
                                col.Item().Text("FACTURA").FontSize(14).SemiBold().AlignCenter();
                                col.Item().Text($"No. {invoice.Sequential}").SemiBold().AlignCenter();
                                col.Item().Text($"Clave de Acceso: {invoice.AccessKey}").FontSize(9);

                                if (!string.IsNullOrWhiteSpace(invoice.AuthorizationNumber))
                                {
                                    col.Item().Text($"Autorización: {invoice.AuthorizationNumber}");
                                }

                                col.Item().Text($"Ambiente: {invoice.Environment}");
                                col.Item().Text($"Fecha Emisión: {FormatDate(invoice.InvoiceDate)}");
                                col.Item().Text($"Fecha Autorización: {FormatDate(invoice.AuthorizationDate)}");
                            });
                        });
                    });

                    header.Item().PaddingTop(10).LineHorizontal(1);
                });

                page.Content().PaddingTop(10).Column(content =>
                {
                    content.Spacing(12);

                    content.Item().Background(Colors.Grey.Lighten4).Padding(12).Column(section =>
                    {
                        section.Spacing(4);
                        section.Item().Text("Datos del Cliente").FontSize(12).SemiBold();
                        section.Item().Text(invoice.Customer?.Name ?? "Cliente no disponible");
                        section.Item().Text($"Documento: {invoice.Customer?.Document} ({invoice.Customer?.DocumentType})");
                        section.Item().Text($"Correo: {invoice.Customer?.Email ?? "N/A"}");
                        section.Item().Text($"Teléfonos: {CombinePhones(invoice.Customer?.Cellphone, invoice.Customer?.Telephone)}");
                        section.Item().Text($"Dirección: {invoice.Customer?.Address ?? "No registrada"}");
                        section.Item().Text($"Forma de pago: {invoice.PaymentMethod} | Plazo: {invoice.PaymentTermDays} días");
                    });

                    content.Item().Column(meta =>
                    {
                        meta.Spacing(4);
                        meta.Item().Text("Resumen").FontSize(12).SemiBold();
                        meta.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Estado: {invoice.Status}");
                            row.RelativeItem().Text($"Documento: {invoice.DocumentType}");
                            row.RelativeItem().Text($"Usuario: {invoice.UserFullName}");
                        });
                        meta.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Fecha emisión: {FormatDate(invoice.InvoiceDate)}");
                            row.RelativeItem().Text($"Fecha vencimiento: {FormatDate(invoice.DueDate)}");
                            row.RelativeItem().Text($"Mensaje SRI: {invoice.SriMessage ?? "Sin observaciones"}");
                        });
                    });

                    content.Item().Text("Detalle de Productos").FontSize(12).SemiBold();

                    content.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(60);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn();
                            columns.ConstantColumn(70);
                            columns.ConstantColumn(70);
                            columns.ConstantColumn(80);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Código").SemiBold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Descripción").SemiBold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Cantidad").SemiBold();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("P. Unitario").SemiBold().AlignRight();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Descuento").SemiBold().AlignRight();
                            header.Cell().Background(Colors.Grey.Lighten2).Padding(5).Text("Total").SemiBold().AlignRight();
                        });

                        foreach (var detail in invoice.Details)
                        {
                            table.Cell().Padding(5).Text(detail.ProductCode);
                            table.Cell().Padding(5).Text($"{detail.ProductName} ({detail.UnitMeasureCode})");
                            table.Cell().Padding(5).Text(detail.Quantity.ToString("0.##"));
                            table.Cell().Padding(5).AlignRight().Text(FormatCurrency(detail.UnitPrice, culture));
                            table.Cell().Padding(5).AlignRight().Text(FormatCurrency(detail.Discount, culture));
                            table.Cell().Padding(5).AlignRight().Text(FormatCurrency(detail.Total, culture));
                        }
                    });

                    content.Item().PaddingTop(6).AlignRight().Column(totals =>
                    {
                        totals.Spacing(2);
                        totals.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight().Text("Subtotal sin impuestos:");
                            row.ConstantItem(120).AlignRight().Text(FormatCurrency(invoice.SubtotalWithoutTaxes, culture));
                        });
                        totals.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight().Text("Subtotal con impuestos:");
                            row.ConstantItem(120).AlignRight().Text(FormatCurrency(invoice.SubtotalWithTaxes, culture));
                        });
                        totals.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight().Text("Descuento:");
                            row.ConstantItem(120).AlignRight().Text(FormatCurrency(invoice.DiscountTotal, culture));
                        });
                        totals.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight().Text("Impuestos:");
                            row.ConstantItem(120).AlignRight().Text(FormatCurrency(invoice.TaxTotal, culture));
                        });
                        totals.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight().Text("Total:").SemiBold();
                            row.ConstantItem(120).AlignRight().Text(FormatCurrency(invoice.TotalInvoice, culture)).FontSize(12).SemiBold();
                        });
                    });

                    content.Item().Background(Colors.Grey.Lighten5).Padding(10).Column(extra =>
                    {
                        extra.Spacing(4);
                        extra.Item().Text("Información adicional").FontSize(12).SemiBold();

                        if (!string.IsNullOrWhiteSpace(invoice.Description))
                        {
                            extra.Item().Text($"Concepto: {invoice.Description}");
                        }

                        if (!string.IsNullOrWhiteSpace(invoice.AdditionalInformation))
                        {
                            extra.Item().Text(invoice.AdditionalInformation);
                        }

                        if (!string.IsNullOrWhiteSpace(invoice.SriMessage))
                        {
                            extra.Item().Text($"Respuesta SRI: {invoice.SriMessage}");
                        }
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Documento generado el ").FontSize(9);
                    text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(9).SemiBold();
                });
            });
        }).GeneratePdf();
    }

    private static string FormatCurrency(decimal value, CultureInfo culture)
        => value.ToString("C2", culture);

    private static string FormatDate(DateTime? date)
        => date.HasValue ? date.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm") : "N/A";

    private static string CombinePhones(string? mobile, string? phone)
    {
        var numbers = new[] { mobile, phone }
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .ToArray();

        return numbers.Length > 0 ? string.Join(" / ", numbers) : "Sin teléfono";
    }
}
