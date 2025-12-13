using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Core.DTOs;
using Core.DTOs.SupplierDto;
using Core.Interfaces.Repository;
using Core.Entities;

namespace Infrastructure.Data;

public class SupplierRepository(StoreContext context, IHttpContextAccessor httpContextAccessor) : ISupplierRepository
{
    public async Task<ApiResponse<SupplierResDto>> CreateSupplierAsync(SupplierCreateReqDto supplierCreateReqDto)
    {
        var response = new ApiResponse<SupplierResDto>();

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

            var existingSupplier = await context.Suppliers
            .FirstOrDefaultAsync(
                c =>
                c.BusinessId == businessId &&
                c.Document == supplierCreateReqDto.Document &&
                c.BusinessName == supplierCreateReqDto.BusinessName);

            if (existingSupplier != null)
            {
                response.Success = false;
                response.Message = "El proveedor ya está registrado en este negocio";
                response.Error = "Error de duplicación";

                return response;
            }

            var newSupplier = new Supplier
            {
                BusinessName = supplierCreateReqDto.BusinessName,
                Document = supplierCreateReqDto.Document,
                Email = supplierCreateReqDto.Email,
                Address = supplierCreateReqDto.Address,
                Cellphone = supplierCreateReqDto.Cellphone,
                Telephone = supplierCreateReqDto.Telephone,
                BusinessId = businessId,
            };

            context.Suppliers.Add(newSupplier);

            await context.SaveChangesAsync();

            var supplier = new SupplierResDto
            {
                Id = newSupplier.Id,
                BusinessName = newSupplier.BusinessName,
                Document = newSupplier.Document,
                Email = newSupplier.Email,
                Address = newSupplier.Address,
                Cellphone = newSupplier.Cellphone,
                Telephone = newSupplier.Telephone,
                IsActive = newSupplier.IsActive,
                CreatedAt = newSupplier.CreatedAt,
            };

            response.Success = true;
            response.Message = "Proveedor creado correctamente";
            response.Data = supplier;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al crear el proveedor";
            response.Error = ex.Message;
        }

        return response;
    }

    public Task<ApiResponse<List<SupplierResDto>>> GetSupplierByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<List<SupplierResDto>>> GetSuppliersAsync(string? keyword, int page, int limit)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<SupplierResDto>> UpdateSupplierAsync(int supplierId, SupplierUpdateReqDto supplierUpdateReqDto)
    {
        throw new NotImplementedException();
    }

    private int GetBusinessIdFromToken()
    {
        var businessIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("BusinessId")?.Value;
        return int.TryParse(businessIdClaim, out var id) ? id : 0;
    }
}
