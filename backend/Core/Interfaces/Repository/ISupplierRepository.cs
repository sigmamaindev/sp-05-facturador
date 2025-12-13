using Core.DTOs;
using Core.DTOs.SupplierDto;

namespace Core.Interfaces.Repository;

public interface ISupplierRepository
{
    Task<ApiResponse<List<SupplierResDto>>> GetSuppliersAsync(string? keyword, int page, int limit);
    Task<ApiResponse<SupplierResDto>> GetSupplierByIdAsync(int id);
    Task<ApiResponse<SupplierResDto>> CreateSupplierAsync(SupplierCreateReqDto supplierCreateReqDto);
    Task<ApiResponse<SupplierResDto>> UpdateSupplierAsync(int supplierId, SupplierUpdateReqDto supplierUpdateReqDto);
}
