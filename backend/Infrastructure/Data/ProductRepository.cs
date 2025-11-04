using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using Core.DTOs;
using Core.DTOs.Product;
using Core.DTOs.Tax;
using Core.DTOs.UnitMeasure;

namespace Infrastructure.Data;

public class ProductRepository(StoreContext context, IHttpContextAccessor httpContextAccessor) : IProductRepository
{
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

    private int GetBusinessIdFromToken()
    {
        var businessIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("BusinessId")?.Value;
        return int.TryParse(businessIdClaim, out var id) ? id : 0;
    }
}
