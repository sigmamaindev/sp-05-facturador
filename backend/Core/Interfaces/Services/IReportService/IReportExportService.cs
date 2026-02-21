using Core.DTOs.ReportDto;

namespace Core.Interfaces.Services.IReportService;

public interface IReportExportService
{
    byte[] GenerateSalesDetailPdf(SalesReportDetailResDto detail);
    byte[] GenerateSalesDetailExcel(SalesReportDetailResDto detail);
    byte[] GeneratePurchasesDetailPdf(PurchasesReportDetailResDto detail);
    byte[] GeneratePurchasesDetailExcel(PurchasesReportDetailResDto detail);
}
