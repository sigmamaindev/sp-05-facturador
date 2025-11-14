using System.Security.Claims;
using Core.Constants;
using Core.DTOs;
using Core.DTOs.Customer;
using Core.DTOs.Invoice;
using Core.DTOs.User;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class InvoiceRepository(StoreContext context, IHttpContextAccessor httpContextAccessor) : IInvoiceRepository
{
    public async Task<ApiResponse<InvoiceResDto>> CreateInvoiceAsync(InvoiceCreateReqDto invoiceCreateReqDto)
    {
        var response = new ApiResponse<InvoiceResDto>();

        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var businessId = GetBusinessIdFromToken();
            var establishmentId = GetEstablishmentIdFromToken();
            var emissionPointId = GetEmissionPointIdFromToken();

            if (businessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no encontrado";
                response.Error = "No existe un negocio con el ID especificado";

                return response;
            }

            if (establishmentId == 0)
            {
                response.Success = false;
                response.Message = "Establecimiento no encontrado";
                response.Error = "No existe un establecimiento con el ID especificado";

                return response;
            }

            var establishment = await context.Establishments
            .FirstOrDefaultAsync(e => e.Id == establishmentId);

            if (emissionPointId == 0)
            {
                response.Success = false;
                response.Message = "Punto de emisión no encontrado";
                response.Error = "No existe un punto de emisión con el ID especificado";

                return response;
            }

            var emissionPoint = await context.EmissionPoints
            .FirstOrDefaultAsync(ep => ep.Id == emissionPointId);

            var userId = GetUserIdFromToken();

            if (userId == 0)
            {
                response.Success = false;
                response.Message = "Usuario no encontrado";
                response.Error = "No existe un usuario con el ID especificado";

                return response;
            }

            var customer = await context.Customers.FindAsync(invoiceCreateReqDto.CustomerId);

            if (customer == null)
            {
                response.Success = false;
                response.Message = "Cliente no encontrado";
                response.Error = "No existe un cliente con el ID especificado";

                return response;
            }

            var lastInvoice = await context.Invoices
               .Where(
                i =>
                i.BusinessId == businessId &&
                i.EmissionPointId == emissionPointId)
               .OrderByDescending(i => i.Id)
               .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastInvoice != null && int.TryParse(lastInvoice.Sequential.Split('-').Last(), out var lastSeq))
            {
                nextNumber = lastSeq + 1;
            }

            var sequence = $"{establishment?.Code}-{emissionPoint?.Code}-{nextNumber:D9}";

            string accesskey = invoiceCreateReqDto.IsElectronic
            ? GenerateAccessKey(invoiceCreateReqDto.InvoiceDate, invoiceCreateReqDto.DocumentType, customer.Document, sequence, invoiceCreateReqDto.Environment)
            : string.Empty;

            var newInvoice = new Invoice
            {
                DocumentType = invoiceCreateReqDto.DocumentType,
                Environment = invoiceCreateReqDto.Environment,
                IsElectronic = invoiceCreateReqDto.IsElectronic,
                InvoiceDate = invoiceCreateReqDto.InvoiceDate,
                DueDate = DateTime.UtcNow,
                CustomerId = customer.Id,
                BusinessId = businessId,
                EstablishmentId = establishmentId,
                EmissionPointId = emissionPointId,
                UserId = userId,
                Sequential = sequence,
                AccessKey = accesskey,
                PaymentMethod = invoiceCreateReqDto.PaymentMethod,
                PaymentTermDays = invoiceCreateReqDto.PaymentTermDays,
                Description = invoiceCreateReqDto.Description,
                AdditionalInformation = invoiceCreateReqDto.AdditionalInformation,
                Status = InvoiceStatuses.DRAFT
            };

            decimal subtotalBase = 0;
            decimal subtotalWithTaxes = 0;
            decimal discountTotal = 0;
            decimal taxTotal = 0;

            foreach (var detail in invoiceCreateReqDto.Details)
            {
                var product = await context.Products
                .Include(p => p.Tax)
                .FirstOrDefaultAsync(p => p.Id == detail.ProductId);

                if (product == null)
                {
                    response.Success = false;
                    response.Message = "Producto no encontrado";
                    response.Error = $"Producto {detail.ProductId} no encontrado";

                    return response;
                }

                var stockRecord = await context.ProductWarehouses
                    .FirstOrDefaultAsync(
                        pw =>
                        pw.ProductId == detail.ProductId &&
                        pw.WarehouseId == detail.WarehouseId);

                if (stockRecord == null || stockRecord.Stock < detail.Quantity)
                {
                    response.Success = false;
                    response.Message = "Producto no encontrado";
                    response.Error = $"Stock insuficiente del producto {product.Name} en bodega";

                    return response;
                }

                var subtotal = detail.Quantity * detail.UnitPrice;
                var discount = detail.Discount;
                var taxableBase = subtotal - discount;

                var taxRate = product.Tax?.Rate ?? 0;
                var taxValue = taxableBase * (taxRate / 100);
                var total = taxableBase + taxValue;

                subtotalBase += taxableBase;
                discountTotal += discount;
                taxTotal += taxValue;
                subtotalWithTaxes += total;

                stockRecord.Stock -= detail.Quantity;

                var newInvoiceDetail = new InvoiceDetail
                {
                    ProductId = detail.ProductId,
                    WarehouseId = detail.WarehouseId,
                    Quantity = detail.Quantity,
                    UnitPrice = detail.UnitPrice,
                    Discount = discount,
                    Subtotal = taxableBase,
                    TaxId = detail.TaxId,
                    TaxRate = taxRate,
                    TaxValue = taxValue,
                    Total = total
                };

                newInvoice.InvoiceDetails.Add(newInvoiceDetail);
            }

            newInvoice.SubtotalWithoutTaxes = subtotalBase;
            newInvoice.SubtotalWithTaxes = subtotalWithTaxes;
            newInvoice.DiscountTotal = discountTotal;
            newInvoice.TaxTotal = taxTotal;
            newInvoice.TotalInvoice = subtotalWithTaxes;

            context.Invoices.Add(newInvoice);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            var invoice = new InvoiceResDto
            {
                Id = newInvoice.Id,
                Sequential = newInvoice.Sequential,
                AccessKey = newInvoice.AccessKey,
                Environment = newInvoice.Environment,
                DocumentType = newInvoice.DocumentType,
                Status = newInvoice.Status,
                IsElectronic = newInvoice.IsElectronic,
                InvoiceDate = newInvoice.InvoiceDate,
                SubtotalWithoutTaxes = newInvoice.SubtotalWithoutTaxes,
                SubtotalWithTaxes = newInvoice.SubtotalWithTaxes,
                DiscountTotal = newInvoice.DiscountTotal,
                TaxTotal = newInvoice.TaxTotal,
                TotalInvoice = newInvoice.TotalInvoice,
                PaymentMethod = newInvoice.PaymentMethod,
                PaymentTermDays = newInvoice.PaymentTermDays,
                Description = newInvoice.Description,
                AdditionalInformation = newInvoice.AdditionalInformation
            };

            response.Success = true;
            response.Message = "Factura creada exitosamente.";
            response.Data = invoice;

            return response;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            response.Success = false;
            response.Message = "Error al crear la factura";
            response.Error = ex.Message;
        }

        return response;
    }

    public Task<ApiResponse<InvoiceResDto>> GetInvoiceByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ApiResponse<List<InvoiceResDto>>> GetInvoicesAsync(string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<InvoiceResDto>>();

        try
        {
            var businessId = GetBusinessIdFromToken();
            var establishmentId = GetEstablishmentIdFromToken();
            var emissionPointId = GetEmissionPointIdFromToken();
            var userId = GetUserIdFromToken();

            if (businessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no encontrado";
                response.Error = "No existe un negocio con el ID especificado";
                return response;
            }

            var query = context.Invoices
                .Include(i => i.Customer)
                .Include(i => i.EmissionPoint)
                .Include(i => i.Establishment)
                .Where(
                    i =>
                    i.BusinessId == businessId &&
                    i.EmissionPointId == emissionPointId &&
                    i.EstablishmentId == establishmentId &&
                    i.UserId == userId);

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(i =>
                    EF.Functions.ILike(i.Sequential, $"%{keyword}%") ||
                    EF.Functions.ILike(i.Customer!.Name, $"%{keyword}%") ||
                    EF.Functions.ILike(i.Customer!.Document, $"%{keyword}%")
                );
            }

            var total = await query.CountAsync();

            var skip = (page - 1) * limit;
            var invoices = await query
                .OrderByDescending(i => i.InvoiceDate)
                .Skip(skip)
                .Take(limit)
                .Select(i => new InvoiceResDto
                {
                    Id = i.Id,
                    Sequential = i.Sequential,
                    AccessKey = i.AccessKey,
                    Environment = i.Environment,
                    DocumentType = i.DocumentType,
                    Status = i.Status,
                    IsElectronic = i.IsElectronic,
                    InvoiceDate = i.InvoiceDate,
                    AuthorizationDate = i.AuthorizationDate,
                    Customer = new CustomerResDto
                    {
                        Id = i.Customer!.Id,
                        DocumentType = i.DocumentType,
                        Document = i.Customer.Document,
                        Name = i.Customer.Name,
                        Email = i.Customer.Email,
                        Address = i.Customer.Address,
                        Cellphone = i.Customer.Cellphone,
                        Telephone = i.Customer.Telephone,
                        IsActive = i.Customer.IsActive,
                        CreatedAt = i.Customer.CreatedAt
                    },
                    User = new UserResDto
                    {
                        Id = i.User!.Id,
                        Document = i.User.Document,
                        FullName = i.User.FullName,
                        Email = i.User.Email,
                        Username = i.User.Username,
                        IsActive = i.User.IsActive,
                        CreatedAt = i.User.CreatedAt
                    },
                    SubtotalWithoutTaxes = i.SubtotalWithoutTaxes,
                    SubtotalWithTaxes = i.SubtotalWithTaxes,
                    DiscountTotal = i.DiscountTotal,
                    TaxTotal = i.TaxTotal,
                    TotalInvoice = i.TotalInvoice,
                    PaymentMethod = i.PaymentMethod,
                    PaymentTermDays = i.PaymentTermDays,
                    Description = i.Description!,
                    AdditionalInformation = i.AdditionalInformation,
                    AuthorizationNumber = i.AuthorizationNumber,
                    SriMessage = i.SriMessage
                }).ToListAsync();

            response.Success = true;
            response.Message = "Listado de facturas obtenido correctamente.";
            response.Data = invoices;
            response.Pagination = new Pagination
            {
                Total = total,
                Page = page,
                Limit = limit
            };

            return response;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener las facturas";
            response.Error = ex.Message;
            return response;
        }
    }

    public Task<ApiResponse<InvoiceResDto>> UpdateInvoiceAsync(int invoiceId, InvoiceUpdateReqDto invoiceUpdateReqDto)
    {
        throw new NotImplementedException();
    }

    private static string GenerateAccessKey(DateTime date, string documentType, string customerId, string sequential, string environment)
    {
        var baseKey = $"{date:ddMMyyyy}{documentType}{customerId.PadLeft(13, '0')}{environment}{sequential.Replace("-", "")}{new Random().Next(10000000, 99999999)}";
        var verifier = (baseKey.Sum(c => c) % 10).ToString();
        return baseKey + verifier;
    }

    private int GetUserIdFromToken()
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var id) ? id : 0;
    }

    private int GetBusinessIdFromToken()
    {
        var businessIdClaim = httpContextAccessor.HttpContext?.User.FindFirst("BusinessId")?.Value;
        return int.TryParse(businessIdClaim, out var id) ? id : 0;
    }

    private int GetEstablishmentIdFromToken()
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
