using Core.DTOs;
using Core.DTOs.Product;

namespace Core.Interfaces;

public interface IProductRepository
{
    Task<ApiResponse<List<ProductResDto>>> GetProductsAsync(string? keyword, int page, int limit);
}
