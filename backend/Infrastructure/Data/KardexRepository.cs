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

    private int GetBusinessIdFromToken()
    {
        var businessIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("BusinessId")?.Value;
        return int.TryParse(businessIdClaim, out var id) ? id : 0;
    }
}
