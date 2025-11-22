using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces.Repository;
using Core.DTOs;
using Core.DTOs.Product;
using Core.DTOs.Tax;
using Core.DTOs.UnitMeasure;
using Core.DTOs.Inventory;
using Core.Entities;

namespace Infrastructure.Data;

public class ProductRepository(StoreContext context, IHttpContextAccessor httpContextAccessor) : IProductRepository
{
    public async Task<ApiResponse<ProductResDto>> CreateProductAsync(ProductCreateReqDto productCreateReqDto)
    {
        var response = new ApiResponse<ProductResDto>();

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
            .FirstOrDefaultAsync(p =>
            p.BusinessId == businessId &&
            p.Sku == productCreateReqDto.Sku &&
            p.Name == productCreateReqDto.Name &&
            p.Description == productCreateReqDto.Description);

            if (existingProduct != null)
            {
                response.Success = false;
                response.Message = "El producto ya está registrado en este negocio";
                response.Error = "Error de duplicación";

                return response;
            }

            var newProduct = new Product
            {
                Sku = productCreateReqDto.Sku,
                Name = productCreateReqDto.Name,
                Description = productCreateReqDto.Description,
                Price = productCreateReqDto.Price,
                Iva = productCreateReqDto.Iva,
                BusinessId = businessId,
                TaxId = productCreateReqDto.TaxId,
                UnitMeasureId = productCreateReqDto.UnitMeasureId,
                Type = productCreateReqDto.Type
            };

            context.Products.Add(newProduct);
            await context.SaveChangesAsync();

            var product = new ProductResDto
            {
                Id = newProduct.Id,
                Sku = newProduct.Sku,
                Name = newProduct.Name,
                Description = newProduct.Description,
                Price = newProduct.Price,
                Iva = newProduct.Iva,
                IsActive = newProduct.IsActive
            };

            response.Success = true;
            response.Message = "Producto creado correctamente";
            response.Data = product;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al crear el producto";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<ProductResDto>> GetProductByIdAsync(int id)
    {
        var response = new ApiResponse<ProductResDto>();

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
            .Include(p => p.Business)
            .Include(p => p.Tax)
            .Include(p => p.UnitMeasure)
            .Include(p => p.ProductWarehouses).ThenInclude(p => p.Warehouse)
            .Where(p => p.IsActive && p.BusinessId == businessId)
            .FirstOrDefaultAsync(p => p.Id == id);

            if (existingProduct == null)
            {
                response.Success = false;
                response.Message = "Producto no encontrado";
                response.Error = "No existe un producto con el ID especificado";

                return response;
            }

            var product = new ProductResDto
            {
                Id = existingProduct.Id,
                Name = existingProduct.Name,
                Description = existingProduct.Description,
                Price = existingProduct.Price,
                Iva = existingProduct.Iva,
                IsActive = existingProduct.IsActive,
                Tax = new TaxResDto
                {
                    Id = existingProduct.Tax!.Id,
                    Code = existingProduct.Tax.Code,
                    CodePercentage = existingProduct.Tax.CodePercentage,
                    Name = existingProduct.Tax.Name,
                    Group = existingProduct.Tax.Group,
                    Rate = existingProduct.Tax.Rate,
                    IsActive = existingProduct.Tax.IsActive
                },
                UnitMeasure = new UnitMeasureResDto
                {
                    Id = existingProduct.UnitMeasure!.Id,
                    Code = existingProduct.UnitMeasure.Code,
                    Name = existingProduct.UnitMeasure.Name,
                    FactorBase = existingProduct.UnitMeasure.FactorBase,
                    IsActive = existingProduct.UnitMeasure.IsActive
                },
                Inventory = [.. existingProduct.ProductWarehouses.Select(pw => new InventoryResDto
                {
                    Id = pw.Id,
                    WarehouseId = pw.Warehouse!.Id,
                    WarehouseCode = pw.Warehouse!.Code,
                    WarehouseName = pw.Warehouse.Name,
                    Stock = pw.Stock,
                    MinStock = pw.MinStock,
                    MaxStock = pw.MaxStock
                })]
            };

            response.Success = true;
            response.Message = "Producto obtenido correctamente";
            response.Data = product;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener el producto";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<List<ProductResDto>>> GetProductsAsync(string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<ProductResDto>>();

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

            var query = context.Products
            .Include(p => p.Business)
            .Include(p => p.Tax)
            .Include(p => p.UnitMeasure)
            .Include(p => p.ProductWarehouses).ThenInclude(p => p.Warehouse)
            .Where(p => p.IsActive && p.BusinessId == businessId);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(
                    p =>
                    EF.Functions.ILike(p.Sku, $"%{keyword}%") ||
                    EF.Functions.ILike(p.Name, $"%{keyword}%") ||
                    EF.Functions.ILike(p.Description, $"%{keyword}%"));
            }

            var total = await query.CountAsync();

            var products = await query
            .OrderBy(p => p.Sku)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(p => new ProductResDto
            {
                Id = p.Id,
                Sku = p.Sku,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Iva = p.Iva,
                IsActive = p.IsActive,
                Tax = new TaxResDto
                {
                    Id = p.Tax!.Id,
                    Code = p.Tax.Code,
                    CodePercentage = p.Tax.CodePercentage,
                    Name = p.Tax.Name,
                    Group = p.Tax.Group,
                    Rate = p.Tax.Rate,
                    IsActive = p.Tax.IsActive
                },
                UnitMeasure = new UnitMeasureResDto
                {
                    Id = p.UnitMeasure!.Id,
                    Code = p.UnitMeasure.Code,
                    Name = p.UnitMeasure.Name,
                    FactorBase = p.UnitMeasure.FactorBase,
                    IsActive = p.UnitMeasure.IsActive
                },
                Inventory = p.ProductWarehouses.Select(pw => new InventoryResDto
                {
                    Id = pw.Id,
                    WarehouseId = pw.Warehouse!.Id,
                    WarehouseCode = pw.Warehouse!.Code,
                    WarehouseName = pw.Warehouse.Name,
                    Stock = pw.Stock,
                    MinStock = pw.MinStock,
                    MaxStock = pw.MaxStock
                }).ToList()
            }).ToListAsync();

            response.Success = true;
            response.Message = "Productos obtenidos correctamente";
            response.Data = products;
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
            response.Message = "Error al obtener los productos";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<ProductResDto>> UpdateProductAsync(int productId, ProductUpdateReqDto productUpdateReqDto)
    {
        var response = new ApiResponse<ProductResDto>();

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
            .Where(p => p.IsActive && p.BusinessId == businessId)
            .FirstOrDefaultAsync(p => p.Id == productId);

            if (existingProduct == null)
            {
                response.Success = false;
                response.Message = "Producto no encontrado";
                response.Error = "No existe un producto con el ID especificado";

                return response;
            }

            existingProduct.Name = productUpdateReqDto.Name;
            existingProduct.Description = productUpdateReqDto.Description;
            existingProduct.Price = productUpdateReqDto.Price;
            existingProduct.Iva = productUpdateReqDto.Iva;
            existingProduct.TaxId = productUpdateReqDto.TaxId;
            existingProduct.UnitMeasureId = productUpdateReqDto.UnitMeasureId;
            existingProduct.Type = productUpdateReqDto.Type;

            await context.SaveChangesAsync();

            var product = new ProductResDto
            {
                Id = existingProduct.Id,
                Name = existingProduct.Name,
                Description = existingProduct.Description,
                Price = existingProduct.Price,
                Iva = existingProduct.Iva,
                IsActive = existingProduct.IsActive
            };

            response.Success = true;
            response.Message = "Producto actualizado correctamente";
            response.Data = product;
        }
        catch (Exception ex)
        {
            response.Success = true;
            response.Message = "Error al actualizar el producto";
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
