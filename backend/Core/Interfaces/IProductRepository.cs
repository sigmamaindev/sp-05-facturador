using Core.DTOs;
using Core.DTOs.Product;

namespace Core.Interfaces;

public interface IProductRepository
{
    Task<ApiResponse<List<ProductResDto>>> GetProductsAsync(string? keyword, int page, int limit);
    Task<ApiResponse<ProductResDto>> GetProductByIdAsync(int id);
    Task<ApiResponse<ProductResDto>> CreateProductAsync(ProductCreateReqDto productCreateReqDto);
    Task<ApiResponse<ProductResDto>> UpdateProductAsync(int productId, ProductUpdateReqDto productUpdateReqDto);
}
