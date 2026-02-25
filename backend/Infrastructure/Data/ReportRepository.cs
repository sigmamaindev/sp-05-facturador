using Microsoft.EntityFrameworkCore;
using Core.DTOs;
using Core.DTOs.ReportDto;
using Core.Interfaces.Repository;
using Core.Interfaces.Services.IUtilService;

namespace Infrastructure.Data;

public class ReportRepository(
    StoreContext context,
    IUserContextService currentUser) : IReportRepository
{
    public async Task<ApiResponse<List<SalesReportResDto>>> GetSalesReportAsync(
        string? keyword,
        int? creditDays,
        DateTime? dateFrom,
        DateTime? dateTo,
        int page,
        int limit)
    {
        var response = new ApiResponse<List<SalesReportResDto>>();

        try
        {
            if (currentUser.BusinessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no encontrado";
                response.Error = "No existe un negocio asociado a este usuario";
                return response;
            }

            var query = context.InvoiceDetails
                .Include(d => d.Invoice).ThenInclude(i => i.Customer)
                .Include(d => d.Invoice).ThenInclude(i => i.User)
                .Include(d => d.Invoice).ThenInclude(i => i.Establishment)
                .Include(d => d.Invoice).ThenInclude(i => i.EmissionPoint)
                .Include(d => d.Product)
                .Where(d => d.Invoice.BusinessId == currentUser.BusinessId)
                .AsQueryable();

            if (!currentUser.IsAdmin)
            {
                query = query.Where(d =>
                    d.Invoice.EstablishmentId == currentUser.EstablishmentId &&
                    d.Invoice.EmissionPointId == currentUser.EmissionPointId &&
                    d.Invoice.UserId == currentUser.UserId);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim();
                query = query.Where(d =>
                    EF.Functions.ILike(d.Invoice.Customer!.Name, $"%{kw}%") ||
                    EF.Functions.ILike(d.Invoice.Customer!.Document, $"%{kw}%"));
            }

            if (creditDays.HasValue)
            {
                query = query.Where(d => d.Invoice.PaymentTermDays == creditDays.Value);
            }

            if (dateFrom.HasValue)
            {
                query = query.Where(d => d.Invoice.InvoiceDate >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                var endOfDay = dateTo.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(d => d.Invoice.InvoiceDate <= endOfDay);
            }

            var total = await query.CountAsync();
            var skip = (page - 1) * limit;

            var details = await query
                .OrderByDescending(d => d.Invoice.InvoiceDate)
                .ThenBy(d => d.Invoice.Id)
                .Skip(skip)
                .Take(limit)
                .Select(d => new SalesReportResDto
                {
                    InvoiceDate = d.Invoice.InvoiceDate,
                    Sequential = (d.Invoice.Establishment != null ? d.Invoice.Establishment.Code : "000") + "-" + (d.Invoice.EmissionPoint != null ? d.Invoice.EmissionPoint.Code : "000") + "-" + d.Invoice.Sequential,
                    CustomerName = d.Invoice.Customer != null ? d.Invoice.Customer.Name : string.Empty,
                    PaymentTermDays = d.Invoice.PaymentTermDays,
                    UserFullName = d.Invoice.User != null ? d.Invoice.User.FullName : string.Empty,
                    ProductName = d.Product != null ? d.Product.Name : string.Empty,
                    Quantity = d.Quantity,
                    GrossWeight = d.GrossWeight,
                    NetWeight = d.NetWeight,
                    UnitPrice = d.UnitPrice,
                    Total = d.Total
                })
                .ToListAsync();

            response.Success = true;
            response.Message = "Reporte de ventas obtenido correctamente";
            response.Data = details;
            response.Pagination = new Pagination
            {
                Total = total,
                Page = page,
                Limit = limit
            };
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener el reporte de ventas";
            response.Error = ex.Message;
        }

        return response;
    }

    // ─── Compras ────────────────────────────────────────────────────────────

    public async Task<ApiResponse<List<PurchasesReportResDto>>> GetPurchasesReportAsync(
        string? keyword,
        DateTime? dateFrom,
        DateTime? dateTo,
        int page,
        int limit)
    {
        var response = new ApiResponse<List<PurchasesReportResDto>>();

        try
        {
            if (currentUser.BusinessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no encontrado";
                response.Error = "No existe un negocio asociado a este usuario";
                return response;
            }

            var query = context.PurchaseDetails
                .Include(d => d.Purchase).ThenInclude(p => p.Supplier)
                .Include(d => d.Purchase).ThenInclude(p => p.User)
                .Include(d => d.Product)
                .Where(d => d.Purchase.BusinessId == currentUser.BusinessId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim();
                query = query.Where(d =>
                    EF.Functions.ILike(d.Purchase.Supplier!.BusinessName, $"%{kw}%") ||
                    EF.Functions.ILike(d.Purchase.Supplier!.Document, $"%{kw}%"));
            }

            if (dateFrom.HasValue)
            {
                query = query.Where(d => d.Purchase.IssueDate >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                var endOfDay = dateTo.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(d => d.Purchase.IssueDate <= endOfDay);
            }

            var total = await query.CountAsync();
            var skip = (page - 1) * limit;

            var details = await query
                .OrderByDescending(d => d.Purchase.IssueDate)
                .ThenBy(d => d.Purchase.Id)
                .Skip(skip)
                .Take(limit)
                .Select(d => new PurchasesReportResDto
                {
                    IssueDate = d.Purchase.IssueDate,
                    Sequential = d.Purchase.EstablishmentCode + "-" + d.Purchase.EmissionPointCode + "-" + d.Purchase.Sequential,
                    SupplierName = d.Purchase.Supplier != null ? d.Purchase.Supplier.BusinessName : string.Empty,
                    UserFullName = d.Purchase.User != null ? d.Purchase.User.FullName : string.Empty,
                    ProductName = d.Product != null ? d.Product.Name : string.Empty,
                    Quantity = d.Quantity,
                    GrossWeight = d.GrossWeight,
                    NetWeight = d.NetWeight,
                    UnitCost = d.UnitCost,
                    Total = d.Total
                })
                .ToListAsync();

            response.Success = true;
            response.Message = "Reporte de compras obtenido correctamente";
            response.Data = details;
            response.Pagination = new Pagination
            {
                Total = total,
                Page = page,
                Limit = limit
            };
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener el reporte de compras";
            response.Error = ex.Message;
        }

        return response;
    }
}
