using System.Globalization;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Core.DTOs.ReportDto;
using Core.Interfaces.Services.IReportService;

namespace Infrastructure.Services.ReportService;

public class ReportExportService : IReportExportService
{
    private static readonly CultureInfo Culture = new("es-EC");

    // ─── PDF ────────────────────────────────────────────────────────────────

    public byte[] GenerateSalesDetailPdf(SalesReportDetailResDto detail)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10));

                // Header
                page.Header().Column(header =>
                {
                    header.Item().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("REPORTE DE VENTA").FontSize(16).SemiBold();
                            col.Item().Text($"Factura N°: {detail.Sequential}").FontSize(12);
                            col.Item().Text($"Fecha: {detail.InvoiceDate:dd/MM/yyyy HH:mm}");
                            col.Item().Text($"Estado: {detail.Status}");
                        });

                        row.ConstantItem(180).Column(col =>
                        {
                            col.Item().Text($"Cliente: {detail.CustomerName}").SemiBold();
                            col.Item().Text($"Documento: {detail.CustomerDocument}");
                            col.Item().Text($"Vendedor: {detail.UserFullName}");
                            col.Item().Text($"Días crédito: {detail.PaymentTermDays}");
                        });
                    });

                    header.Item().PaddingTop(8).LineHorizontal(1);
                });

                // Content
                page.Content().PaddingTop(12).Column(content =>
                {
                    content.Spacing(12);

                    // Products table
                    content.Item().Text("Detalle de Productos").FontSize(11).SemiBold();

                    content.Item().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(55);  // Código
                            cols.RelativeColumn(3);   // Descripción
                            cols.RelativeColumn();    // Unidad
                            cols.ConstantColumn(55);  // Cantidad
                            cols.ConstantColumn(70);  // P. Unitario
                            cols.ConstantColumn(65);  // Descuento
                            cols.ConstantColumn(55);  // IVA %
                            cols.ConstantColumn(65);  // IVA $
                            cols.ConstantColumn(70);  // Total
                        });

                        // Header row
                        static IContainer HeaderCell(IContainer c) =>
                            c.Background(Colors.Blue.Lighten4).Padding(4);

                        table.Header(h =>
                        {
                            h.Cell().Element(HeaderCell).Text("Código").SemiBold();
                            h.Cell().Element(HeaderCell).Text("Descripción").SemiBold();
                            h.Cell().Element(HeaderCell).Text("Unidad").SemiBold();
                            h.Cell().Element(HeaderCell).AlignRight().Text("Cantidad").SemiBold();
                            h.Cell().Element(HeaderCell).AlignRight().Text("P. Unit.").SemiBold();
                            h.Cell().Element(HeaderCell).AlignRight().Text("Desc.").SemiBold();
                            h.Cell().Element(HeaderCell).AlignRight().Text("IVA %").SemiBold();
                            h.Cell().Element(HeaderCell).AlignRight().Text("IVA $").SemiBold();
                            h.Cell().Element(HeaderCell).AlignRight().Text("Total").SemiBold();
                        });

                        var rowIndex = 0;
                        foreach (var item in detail.Items)
                        {
                            var bg = rowIndex % 2 == 0 ? Colors.White : Colors.Grey.Lighten5;
                            rowIndex++;

                            static IContainer DataCell(IContainer c, string color) =>
                                c.Background(color).Padding(4);

                            table.Cell().Element(c => DataCell(c, bg)).Text(item.ProductCode);
                            table.Cell().Element(c => DataCell(c, bg)).Text(item.ProductName);
                            table.Cell().Element(c => DataCell(c, bg)).Text(item.UnitMeasureName);
                            table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(item.Quantity.ToString("0.##"));
                            table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(FormatCurrency(item.UnitPrice));
                            table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(FormatCurrency(item.Discount));
                            table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text($"{item.TaxRate:0.##}%");
                            table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(FormatCurrency(item.TaxValue));
                            table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(FormatCurrency(item.Total));
                        }
                    });

                    // Totals
                    content.Item().AlignRight().Column(totals =>
                    {
                        totals.Spacing(3);

                        TotalRow(totals, "Subtotal sin impuestos:", detail.SubtotalWithoutTaxes);
                        TotalRow(totals, "Subtotal con impuestos:", detail.SubtotalWithTaxes);
                        TotalRow(totals, "Descuento:", detail.DiscountTotal);
                        TotalRow(totals, "Impuestos:", detail.TaxTotal);

                        totals.Item().LineHorizontal(1);

                        totals.Item().Row(row =>
                        {
                            row.RelativeItem().AlignRight().Text("TOTAL:").FontSize(12).SemiBold();
                            row.ConstantItem(110).AlignRight().Text(FormatCurrency(detail.TotalInvoice)).FontSize(12).SemiBold();
                        });
                    });
                });

                // Footer
                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Reporte generado el ").FontSize(9).FontColor(Colors.Grey.Medium);
                    text.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontSize(9).SemiBold();
                });
            });
        }).GeneratePdf();
    }

    // ─── Excel ──────────────────────────────────────────────────────────────

    public byte[] GenerateSalesDetailExcel(SalesReportDetailResDto detail)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Detalle Venta");

        // ── Encabezado de la venta ──────────────────────────────────────────
        ws.Cell(1, 1).Value = "REPORTE DE VENTA";
        ws.Range(1, 1, 1, 9).Merge().Style
            .Font.SetBold(true)
            .Font.SetFontSize(14)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Fill.SetBackgroundColor(XLColor.FromHtml("#1E3A5F"));
        ws.Cell(1, 1).Style.Font.SetFontColor(XLColor.White);

        var infoLabels = new (int Row, string Label, string Value)[]
        {
            (3, "Factura N°:", detail.Sequential),
            (4, "Fecha:", detail.InvoiceDate.ToString("dd/MM/yyyy HH:mm")),
            (5, "Estado:", detail.Status),
            (6, "Cliente:", detail.CustomerName),
            (7, "Documento:", detail.CustomerDocument),
            (8, "Vendedor:", detail.UserFullName),
            (9, "Días crédito:", detail.PaymentTermDays.ToString()),
        };

        foreach (var (row, label, value) in infoLabels)
        {
            ws.Cell(row, 1).Value = label;
            ws.Cell(row, 1).Style.Font.SetBold(true);
            ws.Cell(row, 2).Value = value;
        }

        // ── Tabla de productos ──────────────────────────────────────────────
        int tableStart = 11;

        var headers = new[]
        {
            "Código", "Descripción", "Unidad", "Cantidad",
            "P. Unitario", "Descuento", "IVA %", "IVA $", "Total"
        };

        for (int col = 1; col <= headers.Length; col++)
        {
            var cell = ws.Cell(tableStart, col);
            cell.Value = headers[col - 1];
            cell.Style
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.FromHtml("#1E3A5F"))
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        }

        int dataRow = tableStart + 1;
        bool altRow = false;

        foreach (var item in detail.Items)
        {
            var rowColor = altRow ? XLColor.FromHtml("#EBF0F7") : XLColor.White;
            altRow = !altRow;

            ws.Cell(dataRow, 1).Value = item.ProductCode;
            ws.Cell(dataRow, 2).Value = item.ProductName;
            ws.Cell(dataRow, 3).Value = item.UnitMeasureName;
            ws.Cell(dataRow, 4).Value = (double)item.Quantity;
            ws.Cell(dataRow, 5).Value = (double)item.UnitPrice;
            ws.Cell(dataRow, 6).Value = (double)item.Discount;
            ws.Cell(dataRow, 7).Value = (double)item.TaxRate;
            ws.Cell(dataRow, 8).Value = (double)item.TaxValue;
            ws.Cell(dataRow, 9).Value = (double)item.Total;

            var dataRange = ws.Range(dataRow, 1, dataRow, 9);
            dataRange.Style.Fill.SetBackgroundColor(rowColor);

            // Numeric format for currency columns
            foreach (int col in new[] { 5, 6, 8, 9 })
                ws.Cell(dataRow, col).Style.NumberFormat.Format = "#,##0.00";

            // Numeric format for tax rate
            ws.Cell(dataRow, 7).Style.NumberFormat.Format = "0.##";

            dataRow++;
        }

        // ── Totales ─────────────────────────────────────────────────────────
        int totalsStart = dataRow + 1;

        var totals = new (string Label, decimal Value)[]
        {
            ("Subtotal sin impuestos:", detail.SubtotalWithoutTaxes),
            ("Subtotal con impuestos:", detail.SubtotalWithTaxes),
            ("Descuento:", detail.DiscountTotal),
            ("Impuestos:", detail.TaxTotal),
            ("TOTAL:", detail.TotalInvoice),
        };

        for (int i = 0; i < totals.Length; i++)
        {
            var (label, value) = totals[i];
            int r = totalsStart + i;
            bool isLastRow = i == totals.Length - 1;

            ws.Cell(r, 7).Value = label;
            ws.Cell(r, 7).Style.Font.SetBold(isLastRow).Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);

            ws.Cell(r, 9).Value = (double)value;
            ws.Cell(r, 9).Style.NumberFormat.Format = "#,##0.00";
            ws.Cell(r, 9).Style.Font.SetBold(isLastRow);

            if (isLastRow)
            {
                ws.Range(r, 7, r, 9).Style.Fill.SetBackgroundColor(XLColor.FromHtml("#1E3A5F"));
                ws.Cell(r, 7).Style.Font.SetFontColor(XLColor.White);
                ws.Cell(r, 9).Style.Font.SetFontColor(XLColor.White);
            }
        }

        // ── Bordes y ajuste de columnas ─────────────────────────────────────
        var tableRange = ws.Range(tableStart, 1, dataRow - 1, 9);
        tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Hair;

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    // ─── Helpers ────────────────────────────────────────────────────────────

    private static string FormatCurrency(decimal value)
        => value.ToString("C2", Culture);

    private static void TotalRow(ColumnDescriptor col, string label, decimal value)
    {
        col.Item().Row(row =>
        {
            row.RelativeItem().AlignRight().Text(label);
            row.ConstantItem(110).AlignRight().Text(value.ToString("C2", new CultureInfo("es-EC")));
        });
    }
}
