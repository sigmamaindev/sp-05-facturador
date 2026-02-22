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

            var query = context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.User)
                .Include(i => i.Establishment)
                .Include(i => i.EmissionPoint)
                .Where(i => i.BusinessId == currentUser.BusinessId)
                .AsQueryable();

            if (!currentUser.IsAdmin)
            {
                query = query.Where(i =>
                    i.EstablishmentId == currentUser.EstablishmentId &&
                    i.EmissionPointId == currentUser.EmissionPointId &&
                    i.UserId == currentUser.UserId);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim();
                query = query.Where(i =>
                    EF.Functions.ILike(i.Customer!.Name, $"%{kw}%") ||
                    EF.Functions.ILike(i.Customer!.Document, $"%{kw}%"));
            }

            if (creditDays.HasValue)
            {
                query = query.Where(i => i.PaymentTermDays == creditDays.Value);
            }

            if (dateFrom.HasValue)
            {
                query = query.Where(i => i.InvoiceDate >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                var endOfDay = dateTo.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(i => i.InvoiceDate <= endOfDay);
            }

            var total = await query.CountAsync();
            var skip = (page - 1) * limit;

            var invoices = await query
                .OrderByDescending(i => i.InvoiceDate)
                .Skip(skip)
                .Take(limit)
                .Select(i => new SalesReportResDto
                {
                    Id = i.Id,
                    InvoiceDate = i.InvoiceDate,
                    Sequential = (i.Establishment != null ? i.Establishment.Code : "000") + "-" + (i.EmissionPoint != null ? i.EmissionPoint.Code : "000") + "-" + i.Sequential,
                    PaymentTermDays = i.PaymentTermDays,
                    UserFullName = i.User != null ? i.User.FullName : string.Empty,
                    CustomerName = i.Customer != null ? i.Customer.Name : string.Empty,
                    CustomerDocument = i.Customer != null ? i.Customer.Document : string.Empty,
                    TotalInvoice = i.TotalInvoice,
                    Status = i.Status
                })
                .ToListAsync();

            response.Success = true;
            response.Message = "Reporte de ventas obtenido correctamente";
            response.Data = invoices;
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

    public async Task<ApiResponse<SalesReportDetailResDto>> GetSalesReportDetailAsync(int invoiceId)
    {
        var response = new ApiResponse<SalesReportDetailResDto>();

        try
        {
            if (currentUser.BusinessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no encontrado";
                response.Error = "No existe un negocio asociado a este usuario";
                return response;
            }

            var detailQuery = context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.User)
                .Include(i => i.Establishment)
                .Include(i => i.EmissionPoint)
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Product)
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.UnitMeasure)
                .Where(i =>
                    i.Id == invoiceId &&
                    i.BusinessId == currentUser.BusinessId);

            if (!currentUser.IsAdmin)
            {
                detailQuery = detailQuery.Where(i =>
                    i.EstablishmentId == currentUser.EstablishmentId &&
                    i.EmissionPointId == currentUser.EmissionPointId &&
                    i.UserId == currentUser.UserId);
            }

            var invoice = await detailQuery.FirstOrDefaultAsync();

            if (invoice == null)
            {
                response.Success = false;
                response.Message = "Venta no encontrada";
                response.Error = "No existe una venta con el ID proporcionado";
                return response;
            }

            response.Success = true;
            response.Message = "Detalle de venta obtenido correctamente";
            response.Data = new SalesReportDetailResDto
            {
                Id = invoice.Id,
                InvoiceDate = invoice.InvoiceDate,
                Sequential = (invoice.Establishment?.Code ?? "000") + "-" + (invoice.EmissionPoint?.Code ?? "000") + "-" + invoice.Sequential,
                PaymentTermDays = invoice.PaymentTermDays,
                UserFullName = invoice.User?.FullName ?? string.Empty,
                CustomerName = invoice.Customer?.Name ?? string.Empty,
                CustomerDocument = invoice.Customer?.Document ?? string.Empty,
                SubtotalWithoutTaxes = invoice.SubtotalWithoutTaxes,
                SubtotalWithTaxes = invoice.SubtotalWithTaxes,
                DiscountTotal = invoice.DiscountTotal,
                TaxTotal = invoice.TaxTotal,
                TotalInvoice = invoice.TotalInvoice,
                Status = invoice.Status,
                Items = invoice.InvoiceDetails.Select(d => new SalesReportDetailItemResDto
                {
                    Id = d.Id,
                    ProductCode = d.Product?.Sku ?? string.Empty,
                    ProductName = d.Product?.Name ?? string.Empty,
                    UnitMeasureName = d.UnitMeasure?.Name ?? string.Empty,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Discount = d.Discount,
                    TaxRate = d.TaxRate,
                    TaxValue = d.TaxValue,
                    Subtotal = d.Subtotal,
                    Total = d.Total
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener el detalle de la venta";
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

            var query = context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.User)
                .Where(p => p.BusinessId == currentUser.BusinessId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var kw = keyword.Trim();
                query = query.Where(p =>
                    EF.Functions.ILike(p.Supplier!.BusinessName, $"%{kw}%") ||
                    EF.Functions.ILike(p.Supplier!.Document, $"%{kw}%"));
            }

            if (dateFrom.HasValue)
            {
                query = query.Where(p => p.IssueDate >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                var endOfDay = dateTo.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(p => p.IssueDate <= endOfDay);
            }

            var total = await query.CountAsync();
            var skip = (page - 1) * limit;

            var purchases = await query
                .OrderByDescending(p => p.IssueDate)
                .Skip(skip)
                .Take(limit)
                .Select(p => new PurchasesReportResDto
                {
                    Id = p.Id,
                    IssueDate = p.IssueDate,
                    Sequential = p.EstablishmentCode + "-" + p.EmissionPointCode + "-" + p.Sequential,
                    UserFullName = p.User != null ? p.User.FullName : string.Empty,
                    SupplierName = p.Supplier != null ? p.Supplier.BusinessName : string.Empty,
                    SupplierDocument = p.Supplier != null ? p.Supplier.Document : string.Empty,
                    TotalPurchase = p.TotalPurchase,
                    Status = p.Status
                })
                .ToListAsync();

            response.Success = true;
            response.Message = "Reporte de compras obtenido correctamente";
            response.Data = purchases;
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

    public async Task<ApiResponse<PurchasesReportDetailResDto>> GetPurchasesReportDetailAsync(int purchaseId)
    {
        var response = new ApiResponse<PurchasesReportDetailResDto>();

        try
        {
            if (currentUser.BusinessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no encontrado";
                response.Error = "No existe un negocio asociado a este usuario";
                return response;
            }

            var purchase = await context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.User)
                .Include(p => p.PurchaseDetails)
                    .ThenInclude(d => d.Product)
                .Include(p => p.PurchaseDetails)
                    .ThenInclude(d => d.UnitMeasure)
                .Include(p => p.PurchaseDetails)
                    .ThenInclude(d => d.Warehouse)
                .Where(p =>
                    p.Id == purchaseId &&
                    p.BusinessId == currentUser.BusinessId)
                .FirstOrDefaultAsync();

            if (purchase == null)
            {
                response.Success = false;
                response.Message = "Compra no encontrada";
                response.Error = "No existe una compra con el ID proporcionado";
                return response;
            }

            response.Success = true;
            response.Message = "Detalle de compra obtenido correctamente";
            response.Data = new PurchasesReportDetailResDto
            {
                Id = purchase.Id,
                IssueDate = purchase.IssueDate,
                Sequential = purchase.EstablishmentCode + "-" + purchase.EmissionPointCode + "-" + purchase.Sequential,
                UserFullName = purchase.User?.FullName ?? string.Empty,
                SupplierName = purchase.Supplier?.BusinessName ?? string.Empty,
                SupplierDocument = purchase.Supplier?.Document ?? string.Empty,
                SubtotalWithoutTaxes = purchase.SubtotalWithoutTaxes,
                SubtotalWithTaxes = purchase.SubtotalWithTaxes,
                DiscountTotal = purchase.DiscountTotal,
                TaxTotal = purchase.TaxTotal,
                TotalPurchase = purchase.TotalPurchase,
                Status = purchase.Status,
                Items = purchase.PurchaseDetails.Select(d => new PurchasesReportDetailItemResDto
                {
                    Id = d.Id,
                    ProductCode = d.Product?.Sku ?? string.Empty,
                    ProductName = d.Product?.Name ?? string.Empty,
                    UnitMeasureName = d.UnitMeasure?.Name ?? string.Empty,
                    WarehouseName = d.Warehouse?.Name ?? string.Empty,
                    Quantity = d.Quantity,
                    UnitCost = d.UnitCost,
                    Discount = d.Discount,
                    TaxRate = d.TaxRate,
                    TaxValue = d.TaxValue,
                    Subtotal = d.Subtotal,
                    Total = d.Total
                }).ToList()
            };
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener el detalle de la compra";
            response.Error = ex.Message;
        }

        return response;
    }
}
