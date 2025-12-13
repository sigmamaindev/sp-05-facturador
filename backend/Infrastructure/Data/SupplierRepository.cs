using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Core.Entities;
using Core.DTOs;
using Core.DTOs.SupplierDto;
using Core.Interfaces.Repository;

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

    public async Task<ApiResponse<SupplierResDto>> GetSupplierByIdAsync(int id)
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
            .FirstOrDefaultAsync(s =>
            s.BusinessId == businessId && s.Id == id);

            if (existingSupplier == null)
            {
                response.Success = false;
                response.Message = "Proveedor no encontrado";
                response.Error = "No existe un proveedor con el ID especificado";

                return response;
            }

            var supplier = new SupplierResDto
            {
                Id = existingSupplier.Id,
                BusinessName = existingSupplier.BusinessName,
                Document = existingSupplier.Document,
                Email = existingSupplier.Email,
                Address = existingSupplier.Address,
                Cellphone = existingSupplier.Cellphone,
                Telephone = existingSupplier.Telephone,
                IsActive = existingSupplier.IsActive,
                CreatedAt = existingSupplier.CreatedAt,
            };

            response.Success = true;
            response.Message = "Proveedor obtenido correctamente";
            response.Data = supplier;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener el proveedor";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<List<SupplierResDto>>> GetSuppliersAsync(string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<SupplierResDto>>();

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

            var query = context.Suppliers
            .Where(s => s.IsActive && s.BusinessId == businessId);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(
                    s =>
                    EF.Functions.ILike(s.Document, $"%{keyword}%") ||
                    EF.Functions.ILike(s.BusinessName, $"%{keyword}%") ||
                    EF.Functions.ILike(s.Address, $"%{keyword}%"));
            }

            var total = await query.CountAsync();

            var suppliers = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(s => new SupplierResDto
            {
                Id = s.Id,
                Document = s.Document,
                BusinessName = s.BusinessName,
                Email = s.Email,
                Address = s.Address,
                Cellphone = s.Cellphone,
                Telephone = s.Telephone,
                IsActive = s.IsActive,
                CreatedAt = s.CreatedAt
            }).ToListAsync();

            response.Success = true;
            response.Message = "Proveedores obtenidos correctamente";
            response.Data = suppliers;
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
            response.Message = "Error al obtener los proveedores";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<SupplierResDto>> UpdateSupplierAsync(int supplierId, SupplierUpdateReqDto supplierUpdateReqDto
)
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
            .FirstOrDefaultAsync(s =>
            s.BusinessId == businessId &&
            s.Id == supplierId);

            if (existingSupplier == null)
            {
                response.Success = false;
                response.Message = "Proveedor no encontrado";
                response.Error = "No existe un proveedor con el ID especificado";

                return response;
            }

            existingSupplier.BusinessName = supplierUpdateReqDto.BusinessName;
            existingSupplier.Document = supplierUpdateReqDto.Document;
            existingSupplier.Email = supplierUpdateReqDto.Email;
            existingSupplier.Address = supplierUpdateReqDto.Address;
            existingSupplier.Cellphone = supplierUpdateReqDto.Cellphone;
            existingSupplier.Telephone = supplierUpdateReqDto.Telephone;
            existingSupplier.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            var supplier = new SupplierResDto
            {
                Id = existingSupplier.Id,
                BusinessName = existingSupplier.BusinessName,
                Document = existingSupplier.Document,
                Email = existingSupplier.Email,
                Address = existingSupplier.Address,
                Cellphone = existingSupplier.Cellphone,
                Telephone = existingSupplier.Telephone,
                IsActive = existingSupplier.IsActive,
                CreatedAt = existingSupplier.CreatedAt,
            };

            response.Success = true;
            response.Message = "Proveedor actualizado correctamente";
            response.Data = supplier;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al actualizar el proveedor";
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
