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

    // ─── Ventas PDF ──────────────────────────────────────────────────────────

    public byte[] GenerateSalesReportPdf(List<SalesReportResDto> data)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(8));

                page.Header().Column(header =>
                {
                    header.Item().Text("REPORTE DE VENTAS").FontSize(14).SemiBold();
                    header.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8).FontColor(Colors.Grey.Medium);
                    header.Item().PaddingTop(6).LineHorizontal(1);
                });

                page.Content().PaddingTop(8).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(62);  // Fecha
                        cols.ConstantColumn(50);  // Id
                        cols.RelativeColumn(2);   // Nombre
                        cols.ConstantColumn(35);  // Crédito
                        cols.RelativeColumn(1.2f);// Vendedor
                        cols.RelativeColumn(1.5f);// Producto
                        cols.ConstantColumn(30);  // Cant
                        cols.ConstantColumn(55);  // P Bruto
                        cols.ConstantColumn(55);  // Merma
                        cols.ConstantColumn(55);  // P Neto
                        cols.ConstantColumn(45);  // Precio
                        cols.ConstantColumn(55);  // Total
                        cols.ConstantColumn(50);  // Promedio
                    });

                    static IContainer HeaderCell(IContainer c) =>
                        c.Background(Colors.Blue.Lighten4).Padding(3);

                    table.Header(h =>
                    {
                        h.Cell().Element(HeaderCell).Text("Fecha").SemiBold();
                        h.Cell().Element(HeaderCell).Text("Id").SemiBold();
                        h.Cell().Element(HeaderCell).Text("Nombre").SemiBold();
                        h.Cell().Element(HeaderCell).AlignRight().Text("Crédito").SemiBold();
                        h.Cell().Element(HeaderCell).Text("Vendedor").SemiBold();
                        h.Cell().Element(HeaderCell).Text("Producto").SemiBold();
                        h.Cell().Element(HeaderCell).AlignRight().Text("Cant").SemiBold();
                        h.Cell().Element(HeaderCell).AlignRight().Text("P Bruto").SemiBold();
                        h.Cell().Element(HeaderCell).AlignRight().Text("Merma").SemiBold();
                        h.Cell().Element(HeaderCell).AlignRight().Text("P Neto").SemiBold();
                        h.Cell().Element(HeaderCell).AlignRight().Text("Precio").SemiBold();
                        h.Cell().Element(HeaderCell).AlignRight().Text("Total").SemiBold();
                        h.Cell().Element(HeaderCell).AlignRight().Text("Promedio").SemiBold();
                    });

                    var rowIndex = 0;
                    foreach (var item in data)
                    {
                        var bg = rowIndex % 2 == 0 ? Colors.White : Colors.Grey.Lighten5;
                        rowIndex++;

                        static IContainer DataCell(IContainer c, string color) =>
                            c.Background(color).Padding(3);

                        var merma = item.GrossWeight - item.NetWeight;
                        var promedio = item.Quantity != 0 ? item.NetWeight / item.Quantity : 0;

                        table.Cell().Element(c => DataCell(c, bg)).Text(item.InvoiceDate.ToString("yyyy-MM-dd"));
                        table.Cell().Element(c => DataCell(c, bg)).Text(item.Sequential);
                        table.Cell().Element(c => DataCell(c, bg)).Text(item.CustomerName);
                        table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(item.PaymentTermDays.ToString());
                        table.Cell().Element(c => DataCell(c, bg)).Text(item.UserFullName);
                        table.Cell().Element(c => DataCell(c, bg)).Text(item.ProductName);
                        table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(item.Quantity.ToString("0.##"));
                        table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text($"Lb {item.GrossWeight:0.00}");
                        table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text($"Lb {merma:0.00}");
                        table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text($"Lb {item.NetWeight:0.00}");
                        table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(FormatCurrency(item.UnitPrice));
                        table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(FormatCurrency(item.Total));
                        table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text($"% {promedio:0.00}");
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    // ─── Ventas Excel ────────────────────────────────────────────────────────

    public byte[] GenerateSalesReportExcel(List<SalesReportResDto> data)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Reporte Ventas");

        // Title
        ws.Cell(1, 1).Value = "REPORTE DE VENTAS";
        ws.Range(1, 1, 1, 13).Merge().Style
            .Font.SetBold(true)
            .Font.SetFontSize(14)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Fill.SetBackgroundColor(XLColor.FromHtml("#1E3A5F"));
        ws.Cell(1, 1).Style.Font.SetFontColor(XLColor.White);

        // Headers
        int tableStart = 3;
        var headers = new[]
        {
            "Fecha", "Id", "Nombre", "Crédito", "Vendedor", "Producto",
            "Cant", "P Bruto", "Merma", "P Neto", "Precio", "Total", "Promedio"
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

        // Data rows
        int dataRow = tableStart + 1;
        bool altRow = false;

        foreach (var item in data)
        {
            var rowColor = altRow ? XLColor.FromHtml("#EBF0F7") : XLColor.White;
            altRow = !altRow;

            var merma = item.GrossWeight - item.NetWeight;
            var promedio = item.Quantity != 0 ? item.NetWeight / item.Quantity : 0;

            ws.Cell(dataRow, 1).Value = item.InvoiceDate.ToString("yyyy-MM-dd");
            ws.Cell(dataRow, 2).Value = item.Sequential;
            ws.Cell(dataRow, 3).Value = item.CustomerName;
            ws.Cell(dataRow, 4).Value = item.PaymentTermDays;
            ws.Cell(dataRow, 5).Value = item.UserFullName;
            ws.Cell(dataRow, 6).Value = item.ProductName;
            ws.Cell(dataRow, 7).Value = (double)item.Quantity;
            ws.Cell(dataRow, 8).Value = (double)item.GrossWeight;
            ws.Cell(dataRow, 9).Value = (double)merma;
            ws.Cell(dataRow, 10).Value = (double)item.NetWeight;
            ws.Cell(dataRow, 11).Value = (double)item.UnitPrice;
            ws.Cell(dataRow, 12).Value = (double)item.Total;
            ws.Cell(dataRow, 13).Value = (double)promedio;

            var dataRange = ws.Range(dataRow, 1, dataRow, 13);
            dataRange.Style.Fill.SetBackgroundColor(rowColor);

            foreach (int col in new[] { 7, 8, 9, 10, 11, 12, 13 })
                ws.Cell(dataRow, col).Style.NumberFormat.Format = "#,##0.00";

            dataRow++;
        }

        // Borders
        if (data.Count > 0)
        {
            var tableRange = ws.Range(tableStart, 1, dataRow - 1, 13);
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Hair;
        }

        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    // ─── Compras PDF ────────────────────────────────────────────────────────

    public byte[] GeneratePurchasesReportPdf(List<PurchasesReportResDto> data)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(8));

                page.Header().Column(header =>
                {
                    header.Item().Text("REPORTE DE COMPRAS").FontSize(14).SemiBold();
                    header.Item().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(8).FontColor(Colors.Grey.Medium);
                    header.Item().PaddingTop(6).LineHorizontal(1);
                });

                page.Content().PaddingTop(8).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(62);  // Fecha
                        cols.ConstantColumn(50);  // Id
                        cols.RelativeColumn(2);   // Nombre
                        cols.RelativeColumn(1.2f);// Comprador
                        cols.RelativeColumn(1.5f);// Producto
                        cols.ConstantColumn(30);  // Cant
                        cols.ConstantColumn(55);  // P Bruto
                        cols.ConstantColumn(55);  // Merma
                        cols.ConstantColumn(55);  // P Neto
                        cols.ConstantColumn(45);  // Precio
                        cols.ConstantColumn(55);  // Total
                        cols.ConstantColumn(50);  // Promedio
                    });

                    static IContainer HeaderCell(IContainer c) =>
                        c.Background(Colors.Green.Lighten4).Padding(3);

                    table.Header(h =>
                    {
                        h.Cell().Element(HeaderCell).Text("Fecha").SemiBold();
                        h.Cell().Element(HeaderCell).Text("Id").SemiBold();
                        h.Cell().Element(HeaderCell).Text("Nombre").SemiBold();
                        h.Cell().Element(HeaderCell).Text("Comprador").SemiBold();
                        h.Cell().Element(HeaderCell).Text("Producto").SemiBold();
                        h.Cell().Element(HeaderCell).AlignRight().Text("Cant").SemiBold();
                        h.Cell().Element(HeaderCell).AlignRight().Text("P Bruto").SemiBold();
                        h.Cell().Element(HeaderCell).AlignRight().Text("Merma").SemiBold();
                        h.Cell().Element(HeaderCell).AlignRight().Text("P Neto").SemiBold();
                        h.Cell().Element(HeaderCell).AlignRight().Text("Precio").SemiBold();
                        h.Cell().Element(HeaderCell).AlignRight().Text("Total").SemiBold();
                        h.Cell().Element(HeaderCell).AlignRight().Text("Promedio").SemiBold();
                    });

                    var rowIndex = 0;
                    foreach (var item in data)
                    {
                        var bg = rowIndex % 2 == 0 ? Colors.White : Colors.Grey.Lighten5;
                        rowIndex++;

                        static IContainer DataCell(IContainer c, string color) =>
                            c.Background(color).Padding(3);

                        var merma = item.GrossWeight - item.NetWeight;
                        var promedio = item.Quantity != 0 ? item.NetWeight / item.Quantity : 0;

                        table.Cell().Element(c => DataCell(c, bg)).Text(item.IssueDate.ToString("yyyy-MM-dd"));
                        table.Cell().Element(c => DataCell(c, bg)).Text(item.Sequential);
                        table.Cell().Element(c => DataCell(c, bg)).Text(item.SupplierName);
                        table.Cell().Element(c => DataCell(c, bg)).Text(item.UserFullName);
                        table.Cell().Element(c => DataCell(c, bg)).Text(item.ProductName);
                        table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(item.Quantity.ToString("0.##"));
                        table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text($"Lb {item.GrossWeight:0.00}");
                        table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text($"Lb {merma:0.00}");
                        table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text($"Lb {item.NetWeight:0.00}");
                        table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(FormatCurrency(item.UnitCost));
                        table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(FormatCurrency(item.Total));
                        table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text($"% {promedio:0.00}");
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    // ─── Compras Excel ──────────────────────────────────────────────────────

    public byte[] GeneratePurchasesReportExcel(List<PurchasesReportResDto> data)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Reporte Compras");

        // Title
        ws.Cell(1, 1).Value = "REPORTE DE COMPRAS";
        ws.Range(1, 1, 1, 12).Merge().Style
            .Font.SetBold(true)
            .Font.SetFontSize(14)
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
            .Fill.SetBackgroundColor(XLColor.FromHtml("#1B5E20"));
        ws.Cell(1, 1).Style.Font.SetFontColor(XLColor.White);

        // Headers
        int tableStart = 3;
        var headers = new[]
        {
            "Fecha", "Id", "Nombre", "Comprador", "Producto",
            "Cant", "P Bruto", "Merma", "P Neto", "Precio", "Total", "Promedio"
        };

        for (int col = 1; col <= headers.Length; col++)
        {
            var cell = ws.Cell(tableStart, col);
            cell.Value = headers[col - 1];
            cell.Style
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(XLColor.FromHtml("#1B5E20"))
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        }

        // Data rows
        int dataRow = tableStart + 1;
        bool altRow = false;

        foreach (var item in data)
        {
            var rowColor = altRow ? XLColor.FromHtml("#E8F5E9") : XLColor.White;
            altRow = !altRow;

            var merma = item.GrossWeight - item.NetWeight;
            var promedio = item.Quantity != 0 ? item.NetWeight / item.Quantity : 0;

            ws.Cell(dataRow, 1).Value = item.IssueDate.ToString("yyyy-MM-dd");
            ws.Cell(dataRow, 2).Value = item.Sequential;
            ws.Cell(dataRow, 3).Value = item.SupplierName;
            ws.Cell(dataRow, 4).Value = item.UserFullName;
            ws.Cell(dataRow, 5).Value = item.ProductName;
            ws.Cell(dataRow, 6).Value = (double)item.Quantity;
            ws.Cell(dataRow, 7).Value = (double)item.GrossWeight;
            ws.Cell(dataRow, 8).Value = (double)merma;
            ws.Cell(dataRow, 9).Value = (double)item.NetWeight;
            ws.Cell(dataRow, 10).Value = (double)item.UnitCost;
            ws.Cell(dataRow, 11).Value = (double)item.Total;
            ws.Cell(dataRow, 12).Value = (double)promedio;

            var dataRange = ws.Range(dataRow, 1, dataRow, 12);
            dataRange.Style.Fill.SetBackgroundColor(rowColor);

            foreach (int col in new[] { 6, 7, 8, 9, 10, 11, 12 })
                ws.Cell(dataRow, col).Style.NumberFormat.Format = "#,##0.00";

            dataRow++;
        }

        // Borders
        if (data.Count > 0)
        {
            var tableRange = ws.Range(tableStart, 1, dataRow - 1, 12);
            tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Hair;
        }

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
