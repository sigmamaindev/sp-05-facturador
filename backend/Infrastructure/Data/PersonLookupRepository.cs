using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Core.DTOs;
using Core.DTOs.PersonLookupDto;
using Core.Interfaces.Repository;
using Core.Interfaces.Services.ISriService;

namespace Infrastructure.Data;

public class PersonLookupRepository(
    StoreContext context,
    IHttpContextAccessor httpContextAccessor,
    ISriPersonLookupService sriPersonLookupService) : IPersonLookupRepository
{
    public async Task<ApiResponse<PersonLookupResDto>> LookupByDocumentAsync(string document)
    {
        var response = new ApiResponse<PersonLookupResDto>();

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

            // 1. Buscar en Clientes
            var customer = await context.Customers
                .FirstOrDefaultAsync(c => c.BusinessId == businessId && c.Document == document && c.IsActive);

            if (customer != null)
            {
                response.Success = true;
                response.Message = "Persona encontrada en clientes";
                response.Data = new PersonLookupResDto
                {
                    Document = customer.Document,
                    Name = customer.Name,
                    Source = "customer"
                };
                return response;
            }

            // 2. Buscar en Proveedores
            var supplier = await context.Suppliers
                .FirstOrDefaultAsync(s => s.BusinessId == businessId && s.Document == document && s.IsActive);

            if (supplier != null)
            {
                response.Success = true;
                response.Message = "Persona encontrada en proveedores";
                response.Data = new PersonLookupResDto
                {
                    Document = supplier.Document,
                    Name = supplier.BusinessName,
                    Source = "supplier"
                };
                return response;
            }

            // 3. Consultar al SRI
            var sriResult = await sriPersonLookupService.LookupByDocumentAsync(document);

            if (sriResult != null && !string.IsNullOrEmpty(sriResult.NombreCompleto))
            {
                response.Success = true;
                response.Message = "Persona encontrada en el SRI";
                response.Data = new PersonLookupResDto
                {
                    Document = sriResult.Identificacion,
                    Name = sriResult.NombreCompleto,
                    Source = "sri"
                };
                return response;
            }

            response.Success = false;
            response.Message = "No se encontró información para este documento";
            response.Error = "Persona no encontrada";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al buscar persona";
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
