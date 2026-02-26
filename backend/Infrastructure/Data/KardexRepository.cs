using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Core.DTOs;
using Core.DTOs.KardexDto;
using Core.Interfaces.Repository;

namespace Infrastructure.Data;

public class KardexRepository(StoreContext context, IHttpContextAccessor httpContextAccessor) : IKardexRepository
{
    public async Task<ApiResponse<List<KardexResDto>>> GetKardexAsync(string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<KardexResDto>>();

        try
        {
            var businessId = GetBusinessIdFromToken();

            if (businessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no asociado a esta usuario";
                response.Error = "Error de asociación";
                return response;
            }

            var query = context.Kardexes
                .AsNoTracking()
                .Include(k => k.Product)
                .Include(k => k.Warehouse)
                .Where(k => k.BusinessId == businessId);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(k =>
                    EF.Functions.ILike(k.Reference, $"%{keyword}%") ||
                    EF.Functions.ILike(k.MovementType, $"%{keyword}%") ||
                    EF.Functions.ILike(k.Product!.Sku, $"%{keyword}%") ||
                    EF.Functions.ILike(k.Product!.Name, $"%{keyword}%") ||
                    EF.Functions.ILike(k.Warehouse!.Code, $"%{keyword}%") ||
                    EF.Functions.ILike(k.Warehouse!.Name, $"%{keyword}%"));
            }

            var total = await query.CountAsync();

            var kardex = await query
                .OrderByDescending(k => k.MovementDate)
                .ThenByDescending(k => k.Id)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(k => new KardexResDto
                {
                    Id = k.Id,
                    ProductId = k.ProductId,
                    ProductSku = k.Product != null ? k.Product.Sku : string.Empty,
                    ProductName = k.Product != null ? k.Product.Name : string.Empty,
                    WarehouseId = k.WarehouseId,
                    WarehouseCode = k.Warehouse != null ? k.Warehouse.Code : string.Empty,
                    WarehouseName = k.Warehouse != null ? k.Warehouse.Name : string.Empty,
                    MovementDate = k.MovementDate,
                    MovementType = k.MovementType,
                    Reference = k.Reference,
                    QuantityIn = k.QuantityIn,
                    QuantityOut = k.QuantityOut,
                    UnitCost = k.UnitCost,
                    TotalCost = k.TotalCost
                })
                .ToListAsync();

            response.Success = true;
            response.Message = "Movimientos de kardex obtenidos correctamente";
            response.Data = kardex;
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
            response.Message = "Error al obtener los movimientos de kardex";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<KardexReportWrapperDto>> GetKardexReportAsync(int productId, DateTime dateFrom, DateTime dateTo)
    {
        var response = new ApiResponse<KardexReportWrapperDto>();

        try
        {
            var businessId = GetBusinessIdFromToken();

            if (businessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no asociado a este usuario";
                response.Error = "Error de asociación";
                return response;
            }

            var business = await context.Businesses
                .AsNoTracking()
                .Where(b => b.Id == businessId)
                .Select(b => new { b.Name, b.Document, b.Address })
                .FirstOrDefaultAsync();

            var product = await context.Products
                .AsNoTracking()
                .Where(p => p.Id == productId && p.BusinessId == businessId)
                .Select(p => new { p.Sku, p.Name })
                .FirstOrDefaultAsync();

            if (product == null)
            {
                response.Success = false;
                response.Message = "Producto no encontrado";
                response.Error = "El producto no existe o no pertenece a este negocio";
                return response;
            }

            // Current real stock from ProductWarehouse (Existencia en ProductoBodega)
            var currentStock = await context.ProductWarehouses
                .AsNoTracking()
                .Where(pw => pw.Product!.BusinessId == businessId && pw.ProductId == productId)
                .SumAsync(pw => pw.Stock);

            // Movements from dateFrom onward (to calculate initial balance by working backwards)
            var movementsFromDate = await context.Kardexes
                .AsNoTracking()
                .Where(k => k.BusinessId == businessId && k.ProductId == productId && k.MovementDate >= dateFrom)
                .GroupBy(k => 1)
                .Select(g => new
                {
                    TotalIn = g.Sum(k => k.QuantityIn),
                    TotalOut = g.Sum(k => k.QuantityOut),
                    TotalValueIn = g.Sum(k => k.QuantityIn > 0 ? k.TotalCost : 0m),
                    TotalValueOut = g.Sum(k => k.QuantityOut > 0 ? k.TotalCost : 0m)
                })
                .FirstOrDefaultAsync();

            // Initial stock = current real stock - net movements from dateFrom onward
            var initialStock = currentStock - (movementsFromDate?.TotalIn ?? 0) + (movementsFromDate?.TotalOut ?? 0);

            // Initial value: calculated from Kardex movements before dateFrom
            var initialValueData = await context.Kardexes
                .AsNoTracking()
                .Where(k => k.BusinessId == businessId && k.ProductId == productId && k.MovementDate < dateFrom)
                .GroupBy(k => 1)
                .Select(g => new
                {
                    TotalValueIn = g.Sum(k => k.QuantityIn > 0 ? k.TotalCost : 0m),
                    TotalValueOut = g.Sum(k => k.QuantityOut > 0 ? k.TotalCost : 0m)
                })
                .FirstOrDefaultAsync();

            var initialValue = (initialValueData?.TotalValueIn ?? 0) - (initialValueData?.TotalValueOut ?? 0);

            // Movements in range
            var endOfDay = dateTo.Date.AddDays(1).AddTicks(-1);
            var movements = await context.Kardexes
                .AsNoTracking()
                .Include(k => k.Warehouse)
                .Where(k => k.BusinessId == businessId && k.ProductId == productId
                         && k.MovementDate >= dateFrom && k.MovementDate <= endOfDay)
                .OrderBy(k => k.MovementDate)
                .ThenBy(k => k.Id)
                .ToListAsync();

            // Build rows
            var rows = new List<KardexReportResDto>();
            var runningStock = initialStock;
            var runningValue = initialValue;

            // Initial balance row
            rows.Add(new KardexReportResDto
            {
                MovementDate = dateFrom,
                MovementType = "SALDO_INICIAL",
                RunningStock = runningStock,
                RunningValue = runningValue
            });

            foreach (var k in movements)
            {
                var entryTotal = k.QuantityIn > 0 ? k.TotalCost : 0;
                var exitTotal = k.QuantityOut > 0 ? k.TotalCost : 0;

                runningStock += k.QuantityIn - k.QuantityOut;
                runningValue += entryTotal - exitTotal;

                rows.Add(new KardexReportResDto
                {
                    MovementDate = k.MovementDate,
                    MovementType = k.MovementType,
                    WarehouseCode = k.Warehouse?.Code ?? string.Empty,
                    Reference = k.Reference,
                    EntryQuantity = k.QuantityIn,
                    EntryCost = k.QuantityIn > 0 ? k.UnitCost : 0,
                    EntryTotal = entryTotal,
                    ExitQuantity = k.QuantityOut,
                    ExitCost = k.QuantityOut > 0 ? k.UnitCost : 0,
                    ExitTotal = exitTotal,
                    RunningStock = runningStock,
                    RunningValue = runningValue
                });
            }

            response.Success = true;
            response.Message = "Reporte de kardex generado correctamente";
            response.Data = new KardexReportWrapperDto
            {
                BusinessName = business?.Name ?? string.Empty,
                BusinessAddress = business?.Address ?? string.Empty,
                BusinessRuc = business?.Document ?? string.Empty,
                ProductSku = product.Sku,
                ProductName = product.Name,
                DateFrom = dateFrom,
                DateTo = dateTo,
                ReportDate = DateTime.Now,
                Movements = rows
            };
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al generar el reporte de kardex";
            response.Error = ex.Message;
        }

        return response;
    }

    private int GetBusinessIdFromToken()
    {
        var businessIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("BusinessId")?.Value;
        return int.TryParse(businessIdClaim, out var id) ? id : 0;
    }
}
