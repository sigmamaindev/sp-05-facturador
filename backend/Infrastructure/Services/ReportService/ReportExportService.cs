using System.Globalization;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Core.DTOs.KardexDto;
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

    // ─── Kardex PDF ─────────────────────────────────────────────────────────

    public byte[] GenerateKardexReportPdf(KardexReportWrapperDto data)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(7));

                page.Header().Column(header =>
                {
                    // Business info
                    header.Item().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(data.BusinessName).FontSize(12).SemiBold();
                            col.Item().Text($"DIRECCION: {data.BusinessAddress}").FontSize(7);
                            col.Item().Text($"RUC: {data.BusinessRuc}").FontSize(7);
                        });
                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().AlignRight().Text($"Fecha: {data.ReportDate:dd/MM/yyyy}").FontSize(7);
                        });
                    });

                    header.Item().PaddingTop(4).Text($"Desde: {data.DateFrom:dd/MM/yyyy}    Hasta: {data.DateTo:dd/MM/yyyy}").FontSize(7).AlignCenter();
                    header.Item().PaddingTop(4).LineHorizontal(1);

                    // Product info
                    header.Item().PaddingTop(4).Column(col =>
                    {
                        col.Item().Text($"Código: {data.ProductSku}").FontSize(7).SemiBold();
                        col.Item().Text($"Nombre: {data.ProductName}").FontSize(7).SemiBold();
                    });
                });

                page.Content().PaddingTop(4).Table(table =>
                {
                    table.ColumnsDefinition(cols =>
                    {
                        cols.ConstantColumn(62);  // Fecha
                        cols.ConstantColumn(50);  // Movimiento
                        cols.ConstantColumn(30);  // Bodega
                        cols.RelativeColumn(1);   // Referencia
                        // ENTRADAS
                        cols.ConstantColumn(42);  // Cantidad
                        cols.ConstantColumn(45);  // Costo
                        cols.ConstantColumn(55);  // Total
                        // SALIDAS
                        cols.ConstantColumn(42);  // Cantidad
                        cols.ConstantColumn(45);  // Costo
                        cols.ConstantColumn(55);  // Total
                        // TOTALES
                        cols.ConstantColumn(50);  // Existencia
                        cols.ConstantColumn(55);  // Saldo
                    });

                    static IContainer GroupHeaderCell(IContainer c) =>
                        c.Background("#E0E0E0").Padding(2).AlignCenter();

                    static IContainer HeaderCell(IContainer c) =>
                        c.Background("#F5F5F5").Padding(2);

                    table.Header(h =>
                    {
                        // Group header row
                        h.Cell().ColumnSpan(4).Element(GroupHeaderCell).Text("").FontSize(7);
                        h.Cell().ColumnSpan(3).Element(GroupHeaderCell).Text("ENTRADAS").SemiBold().FontSize(7);
                        h.Cell().ColumnSpan(3).Element(GroupHeaderCell).Text("SALIDAS").SemiBold().FontSize(7);
                        h.Cell().ColumnSpan(2).Element(GroupHeaderCell).Text("TOTALES").SemiBold().FontSize(7);

                        // Column header row
                        h.Cell().Element(HeaderCell).Text("Fecha").SemiBold();
                        h.Cell().Element(HeaderCell).Text("Movimiento").SemiBold();
                        h.Cell().Element(HeaderCell).Text("Bodega").SemiBold();
                        h.Cell().Element(HeaderCell).Text("Referencia").SemiBold();

                        h.Cell().Element(HeaderCell).AlignRight().Text("Cantidad").SemiBold();
                        h.Cell().Element(HeaderCell).AlignRight().Text("Costo").SemiBold();
                        h.Cell().Element(HeaderCell).AlignRight().Text("Total").SemiBold();

                        h.Cell().Element(HeaderCell).AlignRight().Text("Cantidad").SemiBold();
                        h.Cell().Element(HeaderCell).AlignRight().Text("Costo").SemiBold();
                        h.Cell().Element(HeaderCell).AlignRight().Text("Total").SemiBold();

                        h.Cell().Element(HeaderCell).AlignRight().Text("Existencia").SemiBold();
                        h.Cell().Element(HeaderCell).AlignRight().Text("Saldo").SemiBold();
                    });

                    var rowIndex = 0;
                    foreach (var item in data.Movements)
                    {
                        var isInitial = item.MovementType == "SALDO_INICIAL";
                        var bg = isInitial ? "#FFF9C4" : (rowIndex % 2 == 0 ? "#FFFFFF" : "#FAFAFA");
                        rowIndex++;

                        static IContainer DataCell(IContainer c, string color) =>
                            c.Background(color).Padding(2);

                        if (isInitial)
                        {
                            table.Cell().ColumnSpan(4).Element(c => DataCell(c, bg))
                                .Text($"Saldo Al: {item.MovementDate:dd/MM/yyyy}").SemiBold();
                            // Empty entry columns
                            table.Cell().Element(c => DataCell(c, bg)).Text("");
                            table.Cell().Element(c => DataCell(c, bg)).Text("");
                            table.Cell().Element(c => DataCell(c, bg)).Text("");
                            // Empty exit columns
                            table.Cell().Element(c => DataCell(c, bg)).Text("");
                            table.Cell().Element(c => DataCell(c, bg)).Text("");
                            table.Cell().Element(c => DataCell(c, bg)).Text("");
                            // Totals
                            table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(item.RunningStock.ToString("0.00"));
                            table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(FormatCurrency(item.RunningValue));
                        }
                        else
                        {
                            table.Cell().Element(c => DataCell(c, bg)).Text(item.MovementDate.ToString("dd/MM/yyyy"));
                            table.Cell().Element(c => DataCell(c, bg)).Text(item.MovementType);
                            table.Cell().Element(c => DataCell(c, bg)).Text(item.WarehouseCode);
                            table.Cell().Element(c => DataCell(c, bg)).Text(item.Reference);

                            // Entradas
                            table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(item.EntryQuantity > 0 ? item.EntryQuantity.ToString("0.00") : "");
                            table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(item.EntryQuantity > 0 ? item.EntryCost.ToString("0.00") : "");
                            table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(item.EntryQuantity > 0 ? FormatCurrency(item.EntryTotal) : "");

                            // Salidas
                            table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(item.ExitQuantity > 0 ? item.ExitQuantity.ToString("0.00") : "");
                            table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(item.ExitQuantity > 0 ? item.ExitCost.ToString("0.00") : "");
                            table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(item.ExitQuantity > 0 ? FormatCurrency(item.ExitTotal) : "");

                            // Totales
                            table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(item.RunningStock.ToString("0.00"));
                            table.Cell().Element(c => DataCell(c, bg)).AlignRight().Text(FormatCurrency(item.RunningValue));
                        }
                    }
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Página ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
            });
        }).GeneratePdf();
    }

    // ─── Kardex Excel ────────────────────────────────────────────────────────

    public byte[] GenerateKardexReportExcel(KardexReportWrapperDto data)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Kardex");

        const int totalCols = 12;
        var headerColor = XLColor.FromHtml("#1E3A5F");

        // Business info
        ws.Cell(1, 1).Value = data.BusinessName;
        ws.Range(1, 1, 1, totalCols).Merge().Style.Font.SetBold(true).Font.SetFontSize(14);

        ws.Cell(2, 1).Value = $"DIRECCION: {data.BusinessAddress}";
        ws.Range(2, 1, 2, totalCols).Merge();

        ws.Cell(3, 1).Value = $"RUC: {data.BusinessRuc}";
        ws.Range(3, 1, 3, totalCols).Merge();

        ws.Cell(4, 1).Value = $"Desde: {data.DateFrom:dd/MM/yyyy}    Hasta: {data.DateTo:dd/MM/yyyy}";
        ws.Range(4, 1, 4, totalCols).Merge().Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        // Product info
        ws.Cell(6, 1).Value = $"Código: {data.ProductSku}";
        ws.Range(6, 1, 6, 4).Merge().Style.Font.SetBold(true);

        ws.Cell(7, 1).Value = $"Nombre: {data.ProductName}";
        ws.Range(7, 1, 7, 6).Merge().Style.Font.SetBold(true);

        // Group headers
        int groupRow = 9;
        ws.Range(groupRow, 1, groupRow, 4).Merge().Style
            .Fill.SetBackgroundColor(XLColor.FromHtml("#E0E0E0"))
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        ws.Cell(groupRow, 5).Value = "ENTRADAS";
        ws.Range(groupRow, 5, groupRow, 7).Merge().Style
            .Font.SetBold(true)
            .Fill.SetBackgroundColor(XLColor.FromHtml("#E0E0E0"))
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        ws.Cell(groupRow, 8).Value = "SALIDAS";
        ws.Range(groupRow, 8, groupRow, 10).Merge().Style
            .Font.SetBold(true)
            .Fill.SetBackgroundColor(XLColor.FromHtml("#E0E0E0"))
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        ws.Cell(groupRow, 11).Value = "TOTALES";
        ws.Range(groupRow, 11, groupRow, 12).Merge().Style
            .Font.SetBold(true)
            .Fill.SetBackgroundColor(XLColor.FromHtml("#E0E0E0"))
            .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        // Column headers
        int headerRow = 10;
        var headers = new[]
        {
            "Fecha", "Movimiento", "Bodega", "Referencia",
            "Cantidad", "Costo", "Total",
            "Cantidad", "Costo", "Total",
            "Existencia", "Saldo"
        };

        for (int col = 1; col <= headers.Length; col++)
        {
            var cell = ws.Cell(headerRow, col);
            cell.Value = headers[col - 1];
            cell.Style
                .Font.SetBold(true)
                .Font.SetFontColor(XLColor.White)
                .Fill.SetBackgroundColor(headerColor)
                .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
        }

        // Data rows
        int dataRow = headerRow + 1;
        bool altRow = false;

        foreach (var item in data.Movements)
        {
            var isInitial = item.MovementType == "SALDO_INICIAL";
            var rowColor = isInitial
                ? XLColor.FromHtml("#FFF9C4")
                : (altRow ? XLColor.FromHtml("#EBF0F7") : XLColor.White);
            altRow = !altRow;

            if (isInitial)
            {
                ws.Cell(dataRow, 1).Value = $"Saldo Al: {item.MovementDate:dd/MM/yyyy}";
                ws.Range(dataRow, 1, dataRow, 4).Merge().Style.Font.SetBold(true);
            }
            else
            {
                ws.Cell(dataRow, 1).Value = item.MovementDate.ToString("dd/MM/yyyy");
                ws.Cell(dataRow, 2).Value = item.MovementType;
                ws.Cell(dataRow, 3).Value = item.WarehouseCode;
                ws.Cell(dataRow, 4).Value = item.Reference;

                if (item.EntryQuantity > 0)
                {
                    ws.Cell(dataRow, 5).Value = (double)item.EntryQuantity;
                    ws.Cell(dataRow, 6).Value = (double)item.EntryCost;
                    ws.Cell(dataRow, 7).Value = (double)item.EntryTotal;
                }

                if (item.ExitQuantity > 0)
                {
                    ws.Cell(dataRow, 8).Value = (double)item.ExitQuantity;
                    ws.Cell(dataRow, 9).Value = (double)item.ExitCost;
                    ws.Cell(dataRow, 10).Value = (double)item.ExitTotal;
                }
            }

            ws.Cell(dataRow, 11).Value = (double)item.RunningStock;
            ws.Cell(dataRow, 12).Value = (double)item.RunningValue;

            var range = ws.Range(dataRow, 1, dataRow, totalCols);
            range.Style.Fill.SetBackgroundColor(rowColor);

            foreach (int col in new[] { 5, 6, 7, 8, 9, 10, 11, 12 })
                ws.Cell(dataRow, col).Style.NumberFormat.Format = "#,##0.00";

            dataRow++;
        }

        // Borders
        if (data.Movements.Count > 0)
        {
            var tableRange = ws.Range(groupRow, 1, dataRow - 1, totalCols);
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
