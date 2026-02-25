using Core.DTOs.ReportDto;

namespace Core.Interfaces.Services.IReportService;

public interface IReportExportService
{
    byte[] GenerateSalesReportPdf(List<SalesReportResDto> data);
    byte[] GenerateSalesReportExcel(List<SalesReportResDto> data);
    byte[] GeneratePurchasesReportPdf(List<PurchasesReportResDto> data);
    byte[] GeneratePurchasesReportExcel(List<PurchasesReportResDto> data);
}
