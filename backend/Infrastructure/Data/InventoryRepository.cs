using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Core.Interfaces.Repository;
using Core.Entities;
using Core.DTOs;
using Core.DTOs.InventoryDto;

namespace Infrastructure.Data;

public class InventoryRepository(StoreContext context, IHttpContextAccessor httpContextAccessor) : IInventoryRepository
{
    public async Task<ApiResponse<List<InventoryResDto>>> CreateInventoryByProductIdAsync(int productId, InventoryCreateReqDto inventoryCreateReqDto)
    {
        var response = new ApiResponse<List<InventoryResDto>>();

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

            var existingProduct = await context.Products
            .Include(p => p.ProductWarehouses)
            .FirstOrDefaultAsync(p =>
            p.Id == productId &&
            p.IsActive &&
            p.BusinessId == businessId);

            if (existingProduct == null)
            {
                response.Success = false;
                response.Message = "Producto no encontrado";
                response.Error = "No existe un producto con el ID especificado";

                return response;
            }

            var validWarehouseIds = await context.Warehouses
            .Where(w => w.BusinessId == businessId && w.IsActive)
            .Select(w => w.Id)
            .ToListAsync();

            var inventoriesToAdd = new List<ProductWarehouse>();

            foreach (var item in inventoryCreateReqDto.Inventories)
            {
                if (!validWarehouseIds.Contains(item.WarehouseId))
                {
                    continue;
                }

                var newInventory = new ProductWarehouse
                {
                    ProductId = productId,
                    WarehouseId = item.WarehouseId,
                    Stock = item.Stock
                };

                inventoriesToAdd.Add(newInventory);
            }

            if (inventoriesToAdd.Count > 0)
            {
                await context.ProductWarehouses.AddRangeAsync(inventoriesToAdd);
            }

            await context.SaveChangesAsync();

            var inventories = await context.ProductWarehouses
            .Include(pw => pw.Warehouse)
            .Where(pw => pw.ProductId == productId)
            .Select(i => new InventoryResDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                WarehouseId = i.WarehouseId,
                WarehouseCode = i.Warehouse!.Code,
                WarehouseName = i.Warehouse.Name,
                Stock = i.Stock,
                MaxStock = i.MaxStock,
                MinStock = i.MinStock
            }).ToListAsync();

            response.Success = true;
            response.Message = "Inventario asignado correctamente";
            response.Data = inventories;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al crear el inventario";
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
