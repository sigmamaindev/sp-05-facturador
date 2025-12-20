using Microsoft.AspNetCore.Http;
using Core.Interfaces.Repository;
using Core.DTOs;
using Core.DTOs.CustomerDto;
using Microsoft.EntityFrameworkCore;
using Core.Entities;

namespace Infrastructure.Data;

public class CustomerRepository(StoreContext context, IHttpContextAccessor httpContextAccessor) : ICustomerRepository
{
    public async Task<ApiResponse<CustomerResDto>> CreateCustomerAsync(CustomerCreateReqDto customerCreateReqDto)
    {
        var response = new ApiResponse<CustomerResDto>();

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

            var existingCustomer = await context.Customers
            .FirstOrDefaultAsync(c =>
            c.BusinessId == businessId &&
            c.Document == customerCreateReqDto.Document &&
            c.Name == customerCreateReqDto.Name);

            if (existingCustomer != null)
            {
                response.Success = false;
                response.Message = "El cliente ya está registrado en este negocio";
                response.Error = "Error de duplicación";

                return response;
            }

            var newCustomer = new Customer
            {
                Name = customerCreateReqDto.Name,
                Document = customerCreateReqDto.Document,
                DocumentType = customerCreateReqDto.DocumentType,
                Email = customerCreateReqDto.Email,
                Address = customerCreateReqDto.Address,
                Cellphone = customerCreateReqDto.Cellphone,
                Telephone = customerCreateReqDto.Telephone,
                BusinessId = businessId,
            };

            context.Customers.Add(newCustomer);
            await context.SaveChangesAsync();

            var customer = new CustomerResDto
            {
                Id = newCustomer.Id,
                Name = newCustomer.Name,
                Document = newCustomer.Document,
                DocumentType = newCustomer.DocumentType,
                Email = newCustomer.Email,
                Address = newCustomer.Address,
                Cellphone = newCustomer.Cellphone,
                Telephone = newCustomer.Telephone,
                IsActive = newCustomer.IsActive,
                CreatedAt = newCustomer.CreatedAt,
            };

            response.Success = true;
            response.Message = "Cliente creado correctamente";
            response.Data = customer;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al crear el cliente";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<CustomerResDto>> GetCustomerByIdAsync(int id)
    {
        var response = new ApiResponse<CustomerResDto>();

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

            var existingCustomer = await context.Customers
            .FirstOrDefaultAsync(c =>
            c.BusinessId == businessId && c.Id == id);

            if (existingCustomer == null)
            {
                response.Success = false;
                response.Message = "Cliente no encontrado";
                response.Error = "No existe un cliente con el ID especificado";

                return response;
            }

            var customer = new CustomerResDto
            {
                Id = existingCustomer.Id,
                Name = existingCustomer.Name,
                Document = existingCustomer.Document,
                DocumentType = existingCustomer.DocumentType,
                Email = existingCustomer.Email,
                Address = existingCustomer.Address,
                Cellphone = existingCustomer.Cellphone,
                Telephone = existingCustomer.Telephone,
                IsActive = existingCustomer.IsActive,
                CreatedAt = existingCustomer.CreatedAt,
            };

            response.Success = true;
            response.Message = "Cliente obtenido correctamente";
            response.Data = customer;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener el cliente";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<List<CustomerResDto>>> GetCustomersAsync(string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<CustomerResDto>>();

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

            var query = context.Customers
            .Where(c => c.IsActive && c.BusinessId == businessId);

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(
                    c =>
                    EF.Functions.ILike(c.Document, $"%{keyword}%") ||
                    EF.Functions.ILike(c.Name, $"%{keyword}%") ||
                    EF.Functions.ILike(c.Address, $"%{keyword}%"));
            }

            var total = await query.CountAsync();

            var customers = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(c => new CustomerResDto
            {
                Id = c.Id,
                Document = c.Document,
                Name = c.Name,
                Email = c.Email,
                Address = c.Address,
                Cellphone = c.Cellphone,
                Telephone = c.Telephone,
                IsActive = c.IsActive,
                DocumentType = c.DocumentType,
                CreatedAt = c.CreatedAt
            }).ToListAsync();

            response.Success = true;
            response.Message = "Clientes obtenidos correctamente";
            response.Data = customers;
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
            response.Message = "Error al obtener los clientes";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<CustomerResDto>> UpdateCustomerAsync(int customerId, CustomerUpdateReqDto customerUpdateReqDto)
    {
        var response = new ApiResponse<CustomerResDto>();

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

            var existingCustomer = await context.Customers
            .FirstOrDefaultAsync(c =>
            c.BusinessId == businessId &&
            c.Id == customerId);

            if (existingCustomer == null)
            {
                response.Success = false;
                response.Message = "Cliente no encontrado";
                response.Error = "No existe un cliente con el ID especificado";

                return response;
            }

            existingCustomer.Name = customerUpdateReqDto.Name;
            existingCustomer.Document = customerUpdateReqDto.Document;
            existingCustomer.DocumentType = customerUpdateReqDto.DocumentType;
            existingCustomer.Email = customerUpdateReqDto.Email;
            existingCustomer.Address = customerUpdateReqDto.Address;
            existingCustomer.Cellphone = customerUpdateReqDto.Cellphone;
            existingCustomer.Telephone = customerUpdateReqDto.Telephone;
            existingCustomer.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            var customer = new CustomerResDto
            {
                Id = existingCustomer.Id,
                Name = existingCustomer.Name,
                Document = existingCustomer.Document,
                DocumentType = existingCustomer.DocumentType,
                Email = existingCustomer.Email,
                Address = existingCustomer.Address,
                Cellphone = existingCustomer.Cellphone,
                Telephone = existingCustomer.Telephone,
                IsActive = existingCustomer.IsActive,
                CreatedAt = existingCustomer.CreatedAt,
            };

            response.Success = true;
            response.Message = "Cliente actualizado correctamente";
            response.Data = customer;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al actualizar el cliente";
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
