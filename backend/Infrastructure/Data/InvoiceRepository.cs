using Core.DTOs;
using Core.DTOs.Business;
using Core.DTOs.Customer;
using Core.DTOs.Invoice;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Data;

public class InvoiceRepository(StoreContext context, IHttpContextAccessor httpContextAccessor) : IInvoiceRepository
{
    public async Task<ApiResponse<InvoiceResDto>> CreateDraftInvoiceAsync(InvoiceCreateReqDto invoiceCreateReqDto)
    {
        var response = new ApiResponse<InvoiceResDto>();

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

            var establishmentId = GetEstablismentIdFromToken();

            if (establishmentId == 0)
            {
                response.Success = false;
                response.Message = "Establecimiento no asociado a esta usuario";
                response.Error = "Error de asociación";

                return response;
            }

            var emissionPointId = GetEmissionPointIdFromToken();

            if (emissionPointId == 0)
            {
                response.Success = false;
                response.Message = "Punto de emisión no asociado a esta usuario";
                response.Error = "Error de asociación";

                return response;
            }

            var invoice = new Invoice
            {
                Sequential = invoiceCreateReqDto.Sequential,
                Subtotal = invoiceCreateReqDto.Subtotal,
                DiscountTotal = invoiceCreateReqDto.DiscountTotal,
                TotalInvoice = invoiceCreateReqDto.TotalInvoice,
                Description = invoiceCreateReqDto.Description,
                BusinessId = businessId,
                EstablishmentId = establishmentId,
                EmissionPointId = emissionPointId,
                CustomerId = invoiceCreateReqDto.CustomerId,
                InvoiceDetails = [.. invoiceCreateReqDto.Details.Select(d=>new InvoiceDetails
                {
                    ProductId = d.ProductId,
                    Description = d.Description,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Discount = d.Discount,
                    Subtotal = d.Subtotal,
                    IvaValue = d.IvaValue,
                    Total = d.Total,
                    TaxRate = d.TaxRate,
                                    })],
                InvoiceTaxTotals = [.. invoiceCreateReqDto.TaxTotals.Select(t => new InvoiceTaxTotal
                {
                    TaxCode = t.TaxCode,
                    TaxRate = t.TaxRate,
                    TaxableBase = t.TaxableBase,
                    TaxValue = t.TaxValue
                })]
            };

            context.Invoices.Add(invoice);
            await context.SaveChangesAsync();

            var invoiceResDto = new InvoiceResDto
            {
                Id = invoice.Id,
                Sequential = invoice.Sequential,
                Subtotal = invoice.Subtotal,
                DiscountTotal = invoice.DiscountTotal,
                TotalInvoice = invoice.TotalInvoice,
                Status = invoice.Status,
                Description = invoice.Description,
                InvoiceDate = invoice.InvoiceDate
            };

            response.Success = true;
            response.Message = "Factura borrador creada correctamente";
            response.Data = invoiceResDto;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al crear la factura borrador";
            response.Error = ex.Message;
        }

        return response;
    }

    private int GetBusinessIdFromToken()
    {
        var businessIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("BusinessId")?.Value;
        return int.TryParse(businessIdClaim, out var id) ? id : 0;
    }

    private int GetEstablismentIdFromToken()
    {
        var establishmentIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("EstablishmentId")?.Value;
        return int.TryParse(establishmentIdClaim, out var id) ? id : 0;
    }

    private int GetEmissionPointIdFromToken()
    {
        var emissionPointIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("EmissionPointId")?.Value;
        return int.TryParse(emissionPointIdClaim, out var id) ? id : 0;
    }
}
