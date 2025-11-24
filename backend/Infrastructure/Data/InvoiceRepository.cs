using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Core.Constants;
using Core.DTOs;
using Core.DTOs.Customer;
using Core.DTOs.Invoice;
using Core.Entities;
using Core.Interfaces.Repository;
using Core.Interfaces.Services;

namespace Infrastructure.Data;

public class InvoiceRepository(StoreContext context, IHttpContextAccessor httpContextAccessor, IInvoiceXmlBuilder invoiceXmlBuilder, IAesEncryptionService aes, IElectronicSignature electronicSignature, ISriReceptionService sriReceptionService) : IInvoiceRepository
{
    public async Task<ApiResponse<InvoiceSimpleResDto>> CreateInvoiceAsync(InvoiceCreateReqDto invoiceCreateReqDto)
    {
        var response = new ApiResponse<InvoiceSimpleResDto>();

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

            if (emissionPointId == 0)
            {
                response.Success = false;
                response.Message = "Punto de emisión no encontrado";
                response.Error = "No existe un punto de emisión con el ID especificado";

                return response;
            }

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

            var business = await context.Businesses
            .FirstOrDefaultAsync(b => b.Id == businessId);

            if (business == null)
            {
                response.Success = false;
                response.Message = "Negocio no encontrado";
                response.Error = "No existe un negocio con el ID especificado";

                return response;
            }

            var establishment = await context.Establishments
            .FirstOrDefaultAsync(e => e.Id == establishmentId);

            if (establishment == null)
            {
                response.Success = false;
                response.Message = "Establecimiento no encontrado";
                response.Error = "No existe un establecimiento con el ID especificado";

                return response;
            }

            var emissionPoint = await context.EmissionPoints
            .FirstOrDefaultAsync(ep => ep.Id == emissionPointId);

            if (emissionPoint == null)
            {
                response.Success = false;
                response.Message = "Punto de emisión no encontrado";
                response.Error = "No existe un punto de emisión con el ID especificado";

                return response;
            }

            var user = await context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                response.Success = false;
                response.Message = "Usuario no encontrado";
                response.Error = "No existe un usuario con el ID especificado";

                return response;
            }

            var lastInvoice = await context.Invoices
               .Where(
                i =>
                i.BusinessId == businessId &&
                i.EstablishmentId == establishmentId &&
                i.EmissionPointId == emissionPointId)
               .OrderByDescending(i => i.Id)
               .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastInvoice != null && int.TryParse(lastInvoice.Sequential, out var lastSeq))
            {
                nextNumber = lastSeq + 1;
            }

            var sequence = $"{nextNumber:D9}";

            var emissionDate = invoiceCreateReqDto.InvoiceDate == default
                ? DateTime.UtcNow
                : invoiceCreateReqDto.InvoiceDate.ToUniversalTime();

            var dueDate = invoiceCreateReqDto.DueDate == default
                ? emissionDate.AddDays(invoiceCreateReqDto.PaymentTermDays)
                : invoiceCreateReqDto.DueDate.ToUniversalTime();

            var newInvoice = new Invoice
            {
                DocumentType = customer.DocumentType,
                Environment = invoiceCreateReqDto.Environment,
                IsElectronic = invoiceCreateReqDto.IsElectronic,
                InvoiceDate = emissionDate,
                DueDate = dueDate,
                CustomerId = customer.Id,
                BusinessId = businessId,
                EstablishmentId = establishmentId,
                EmissionPointId = emissionPointId,
                UserId = userId,
                Sequential = sequence,
                AccessKey = "",
                PaymentMethod = invoiceCreateReqDto.PaymentMethod,
                PaymentTermDays = invoiceCreateReqDto.PaymentTermDays,
                Description = invoiceCreateReqDto.Description,
                AdditionalInformation = invoiceCreateReqDto.AdditionalInformation,
                Status = InvoiceStatuses.DRAFT,
                Business = business,
                Establishment = establishment,
                EmissionPoint = emissionPoint,
                Customer = customer
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

                var warehouse = await context.Warehouses
                .FirstOrDefaultAsync(
                    w =>
                    w.Id == detail.WarehouseId);

                if (warehouse == null)
                {
                    response.Success = false;
                    response.Message = "Bodega no encontrada";
                    response.Error = $"No existe una bodega con el ID especificado";

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
                    response.Error = $"Stock insuficiente del producto {product.Name} en bodega {warehouse.Code}";

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
                    Total = total,
                    Product = product,
                    Warehouse = warehouse,
                    Tax = product.Tax
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

            if (newInvoice.IsElectronic)
            {
                newInvoice.AccessKey = GenerateAccessKey(
                    newInvoice.InvoiceDate,
                    newInvoice.DocumentType,
                    business.Document,
                    newInvoice.Environment,
                    establishment.Code,
                    emissionPoint.Code,
                    newInvoice.Sequential
                );
                var unsignedXml = invoiceXmlBuilder.BuildXMLInvoice(newInvoice, business, establishment, emissionPoint, customer);

                var (pfxBytes, pfxPassword) = await GetCertificateAsync(businessId);

                var signedXml = await electronicSignature.SignXmlAsync(
                    unsignedXml,
                    pfxBytes,
                    pfxPassword,
                    cancellationToken: default
                );

                newInvoice.XmlSigned = signedXml;
                newInvoice.Status = InvoiceStatuses.SIGNED;

                var receptionResponse = await sriReceptionService.SendInvoiceSriAsync(signedXml, newInvoice.Environment == "2");

                if (receptionResponse.State == InvoiceStatuses.SRI_RECEIVED)
                {
                    newInvoice.Status = InvoiceStatuses.SRI_RECEIVED;
                    newInvoice.SriMessage = receptionResponse.Message;
                }
                else if (receptionResponse.State == InvoiceStatuses.SRI_REJECTED)
                {
                    newInvoice.Status = InvoiceStatuses.SRI_REJECTED;
                    newInvoice.SriMessage = receptionResponse.Message;
                }
                else if (receptionResponse.State == InvoiceStatuses.SRI_RETURNED)
                {
                    newInvoice.Status = InvoiceStatuses.SRI_RETURNED;
                    newInvoice.SriMessage = receptionResponse.Message;
                }
                else if (receptionResponse.State is InvoiceStatuses.SRI_TIMEOUT or InvoiceStatuses.SRI_UNAVAILABLE)
                {
                    newInvoice.Status = InvoiceStatuses.SRI_UNAVAILABLE;
                }
                else
                {
                    newInvoice.Status = InvoiceStatuses.ERROR;
                }
            }
            else
            {
                newInvoice.AccessKey = string.Empty;
                newInvoice.XmlSigned = null;
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            var invoice = new InvoiceSimpleResDto
            {
                Id = newInvoice.Id,
                Sequential = newInvoice.Sequential,
                AccessKey = newInvoice.AccessKey,
                AuthorizationNumber = newInvoice.AuthorizationNumber,
                Environment = newInvoice.Environment,
                DocumentType = customer.DocumentType,
                Status = newInvoice.Status,
                IsElectronic = newInvoice.IsElectronic,
                InvoiceDate = newInvoice.InvoiceDate,
                DueDate = newInvoice.DueDate,
                BusinessId = business!.Id,
                BusinessDocument = business.Document,
                BusinessName = business.Name,
                EstablishmentId = establishment!.Id,
                EstablishmentCode = establishment.Code,
                EstablishmentName = establishment.Name,
                EmissionPointId = emissionPoint!.Id,
                EmissionPointCode = emissionPoint.Code,
                EmissionPointDescription = emissionPoint.Description,
                UserId = user!.Id,
                UserDocument = user.Document,
                UserFullName = user.FullName,
                Customer = new CustomerResDto
                {
                    Id = customer.Id,
                    DocumentType = customer.DocumentType,
                    Document = customer.Document,
                    Email = customer.Email,
                    Name = customer.Name,
                    Cellphone = customer.Cellphone,
                    Telephone = customer.Telephone,
                    Address = customer.Address,
                    IsActive = customer.IsActive,
                    CreatedAt = customer.CreatedAt
                },
                SubtotalWithoutTaxes = newInvoice.SubtotalWithoutTaxes,
                SubtotalWithTaxes = newInvoice.SubtotalWithTaxes,
                DiscountTotal = newInvoice.DiscountTotal,
                TaxTotal = newInvoice.TaxTotal,
                TotalInvoice = newInvoice.TotalInvoice,
                PaymentMethod = newInvoice.PaymentMethod,
                PaymentTermDays = newInvoice.PaymentTermDays,
                Description = newInvoice.Description,
                AdditionalInformation = newInvoice.AdditionalInformation,
                AuthorizationDate = newInvoice.AuthorizationDate,
                SriMessage = newInvoice.SriMessage,
                XmlSigned = newInvoice.XmlSigned ?? ""
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

    public async Task<ApiResponse<InvoiceComplexResDto>> GetInvoiceByIdAsync(int id)
    {
        var response = new ApiResponse<InvoiceComplexResDto>();

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

            var existingInvoice = await context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Business)
            .Include(i => i.Establishment)
            .Include(i => i.EmissionPoint)
            .Include(i => i.User)
            .Include(i => i.InvoiceDetails)
            .ThenInclude(d => d.Product)
            .ThenInclude(p => p!.UnitMeasure)
            .Include(i => i.InvoiceDetails).ThenInclude(d => d.Warehouse)
            .Include(i => i.InvoiceDetails).ThenInclude(d => d.Tax)
            .FirstOrDefaultAsync(i => i.BusinessId == businessId && i.Id == id);

            if (existingInvoice == null)
            {
                response.Success = false;
                response.Message = "Factura no encontrada";
                response.Error = "No existe una Factura con el ID especificado";

                return response;
            }

            var invoice = new InvoiceComplexResDto
            {
                Id = existingInvoice.Id,
                Sequential = existingInvoice.Sequential,
                AccessKey = existingInvoice.AccessKey,
                AuthorizationNumber = existingInvoice.AuthorizationNumber,
                Environment = existingInvoice.Environment,
                DocumentType = existingInvoice.DocumentType,
                Status = existingInvoice.Status,
                IsElectronic = existingInvoice.IsElectronic,
                InvoiceDate = existingInvoice.InvoiceDate,
                DueDate = existingInvoice.DueDate,
                BusinessId = existingInvoice.Business!.Id,
                BusinessDocument = existingInvoice.Business.Document,
                BusinessName = existingInvoice.Business.Name,
                EstablishmentId = existingInvoice.Establishment!.Id,
                EstablishmentCode = existingInvoice.Establishment.Code,
                EstablishmentName = existingInvoice.Establishment.Name,
                EmissionPointId = existingInvoice.EmissionPoint!.Id,
                EmissionPointCode = existingInvoice.EmissionPoint.Code,
                EmissionPointDescription = existingInvoice.EmissionPoint.Description,
                UserId = existingInvoice.User!.Id,
                UserDocument = existingInvoice.User.Document,
                UserFullName = existingInvoice.User.FullName,
                Customer = new CustomerResDto
                {
                    Id = existingInvoice.Customer!.Id,
                    DocumentType = existingInvoice.Customer.DocumentType,
                    Document = existingInvoice.Customer.Document,
                    Email = existingInvoice.Customer.Email,
                    Name = existingInvoice.Customer.Name,
                    Cellphone = existingInvoice.Customer.Cellphone,
                    Telephone = existingInvoice.Customer.Telephone,
                    Address = existingInvoice.Customer.Address,
                    IsActive = existingInvoice.Customer.IsActive,
                    CreatedAt = existingInvoice.Customer.CreatedAt
                },
                SubtotalWithoutTaxes = existingInvoice.SubtotalWithoutTaxes,
                SubtotalWithTaxes = existingInvoice.SubtotalWithTaxes,
                DiscountTotal = existingInvoice.DiscountTotal,
                TaxTotal = existingInvoice.TaxTotal,
                TotalInvoice = existingInvoice.TotalInvoice,
                PaymentMethod = existingInvoice.PaymentMethod,
                PaymentTermDays = existingInvoice.PaymentTermDays,
                Description = existingInvoice.Description ?? "",
                AdditionalInformation = existingInvoice.AdditionalInformation,
                AuthorizationDate = existingInvoice.AuthorizationDate,
                SriMessage = existingInvoice.SriMessage,
                XmlSigned = existingInvoice.XmlSigned ?? "",
                Details = [.. existingInvoice.InvoiceDetails.Select(d => new InvoiceDetailResDto
                {
                    Id = d.Id,
                    InvoiceId = d.InvoiceId,
                    ProductId = d.Product!.Id,
                    ProductCode = d.Product.Sku,
                    ProductName = d.Product.Name,
                    UnitMeasureId = d.Product.UnitMeasureId,
                    UnitMeasureCode = d.Product.UnitMeasure!.Code,
                    UnitMeasureName = d.Product.UnitMeasure!.Name,
                    WarehouseId = d.Warehouse!.Id,
                    WarehouseCode = d.Warehouse.Code,
                    WarehouseName = d.Warehouse.Name,
                    TaxId = d.Tax!.Id,
                    TaxCode = d.Tax.Code,
                    TaxName = d.Tax.Name,
                    TaxRate = d.TaxRate,
                    TaxValue = d.TaxValue,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Discount = d.Discount,
                    Subtotal = d.Subtotal,
                    Total = d.Total
                })]
            };

            response.Success = true;
            response.Message = "Factura obtenida correctamente";
            response.Data = invoice;

        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener la factura";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<List<InvoiceSimpleResDto>>> GetInvoicesAsync(string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<InvoiceSimpleResDto>>();

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
                .Select(i => new InvoiceSimpleResDto
                {
                    Id = i.Id,
                    Sequential = i.Sequential,
                    AccessKey = i.AccessKey,
                    AuthorizationNumber = i.AuthorizationNumber,
                    Environment = i.Environment,
                    DocumentType = i.DocumentType,
                    Status = i.Status,
                    IsElectronic = i.IsElectronic,
                    InvoiceDate = i.InvoiceDate,
                    DueDate = i.DueDate,
                    BusinessId = i.Business!.Id,
                    BusinessDocument = i.Business.Document,
                    BusinessName = i.Business.Name,
                    EstablishmentId = i.Establishment!.Id,
                    EstablishmentCode = i.Establishment.Code,
                    EstablishmentName = i.Establishment.Name,
                    EmissionPointId = i.EmissionPoint!.Id,
                    EmissionPointCode = i.EmissionPoint.Code,
                    EmissionPointDescription = i.EmissionPoint.Description,
                    UserId = i.User!.Id,
                    UserDocument = i.User.Document,
                    UserFullName = i.User.FullName,
                    Customer = new CustomerResDto
                    {
                        Id = i.Customer!.Id,
                        DocumentType = i.Customer.DocumentType,
                        Document = i.Customer.Document,
                        Email = i.Customer.Email,
                        Name = i.Customer.Name,
                        Cellphone = i.Customer.Cellphone,
                        Telephone = i.Customer.Telephone,
                        Address = i.Customer.Address,
                        IsActive = i.Customer.IsActive,
                        CreatedAt = i.Customer.CreatedAt
                    },
                    SubtotalWithoutTaxes = i.SubtotalWithoutTaxes,
                    SubtotalWithTaxes = i.SubtotalWithTaxes,
                    DiscountTotal = i.DiscountTotal,
                    TaxTotal = i.TaxTotal,
                    TotalInvoice = i.TotalInvoice,
                    PaymentMethod = i.PaymentMethod,
                    PaymentTermDays = i.PaymentTermDays,
                    Description = i.Description ?? "",
                    AdditionalInformation = i.AdditionalInformation,
                    AuthorizationDate = i.AuthorizationDate,
                    SriMessage = i.SriMessage,
                    XmlSigned = i.XmlSigned ?? ""
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

    public async Task<ApiResponse<InvoiceComplexResDto>> UpdateInvoiceAsync(int invoiceId, InvoiceUpdateReqDto invoiceUpdateReqDto)
    {
        var response = new ApiResponse<InvoiceComplexResDto>();

        using var transaction = await context.Database.BeginTransactionAsync();

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

            if (establishmentId == 0)
            {
                response.Success = false;
                response.Message = "Establecimiento no encontrado";
                response.Error = "No existe un establecimiento con el ID especificado";

                return response;
            }

            if (emissionPointId == 0)
            {
                response.Success = false;
                response.Message = "Punto de emisión no encontrado";
                response.Error = "No existe un punto de emisión con el ID especificado";

                return response;
            }

            if (userId == 0)
            {
                response.Success = false;
                response.Message = "Usuario no encontrado";
                response.Error = "No existe un usuario con el ID especificado";

                return response;
            }

            var invoice = await context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Business)
            .Include(i => i.Establishment)
            .Include(i => i.EmissionPoint)
            .Include(i => i.User)
            .Include(i => i.InvoiceDetails)
            .ThenInclude(d => d.Product)
            .ThenInclude(p => p!.UnitMeasure)
            .Include(i => i.InvoiceDetails).ThenInclude(d => d.Warehouse)
            .Include(i => i.InvoiceDetails).ThenInclude(d => d.Tax)
            .FirstOrDefaultAsync(
                i =>
                i.Id == invoiceId &&
                i.BusinessId == businessId &&
                i.EstablishmentId == establishmentId &&
                i.EmissionPointId == emissionPointId &&
                i.UserId == userId);

            if (invoice == null)
            {
                response.Success = false;
                response.Message = "Factura no encontrada";
                response.Error = "No existe una Factura con el ID especificado";

                return response;
            }

            if (invoice.Customer == null || invoice.Business == null || invoice.Establishment == null || invoice.EmissionPoint == null)
            {
                response.Success = false;
                response.Message = "Datos incompletos para la factura";
                response.Error = "No se encontraron datos de negocio, establecimiento, punto de emisión o cliente asociados";

                return response;
            }

            var emissionDate = invoiceUpdateReqDto.InvoiceDate == default
                ? DateTime.UtcNow
                : invoiceUpdateReqDto.InvoiceDate.ToUniversalTime();

            var dueDate = invoiceUpdateReqDto.DueDate == default
                ? emissionDate.AddDays(invoiceUpdateReqDto.PaymentTermDays)
                : invoiceUpdateReqDto.DueDate.ToUniversalTime();

            invoice.DocumentType = invoiceUpdateReqDto.DocumentType;
            invoice.IsElectronic = invoiceUpdateReqDto.IsElectronic;
            invoice.Environment = invoiceUpdateReqDto.Environment;
            invoice.InvoiceDate = emissionDate;
            invoice.DueDate = dueDate;
            invoice.PaymentMethod = invoiceUpdateReqDto.PaymentMethod;
            invoice.PaymentTermDays = invoiceUpdateReqDto.PaymentTermDays;
            invoice.Description = invoiceUpdateReqDto.Description;
            invoice.AdditionalInformation = invoiceUpdateReqDto.AdditionalInformation;

            decimal subtotalBase = 0;
            decimal subtotalWithTaxes = 0;
            decimal discountTotal = 0;
            decimal taxTotal = 0;

            foreach (var detail in invoice.InvoiceDetails)
            {
                var taxableBase = (detail.Quantity * detail.UnitPrice) - detail.Discount;
                var taxRate = detail.Tax?.Rate ?? detail.TaxRate;
                var taxValue = taxableBase * (taxRate / 100);
                var total = taxableBase + taxValue;

                detail.Subtotal = taxableBase;
                detail.TaxRate = taxRate;
                detail.TaxValue = taxValue;
                detail.Total = total;

                subtotalBase += taxableBase;
                discountTotal += detail.Discount;
                taxTotal += taxValue;
                subtotalWithTaxes += total;
            }

            invoice.SubtotalWithoutTaxes = subtotalBase;
            invoice.SubtotalWithTaxes = subtotalWithTaxes;
            invoice.DiscountTotal = discountTotal;
            invoice.TaxTotal = taxTotal;
            invoice.TotalInvoice = subtotalWithTaxes;

            if (invoice.IsElectronic)
            {
                invoice.AccessKey = GenerateAccessKey(invoice.InvoiceDate, invoice.DocumentType, invoice.Business.Document, invoice.Environment, invoice.Establishment.Code, invoice.EmissionPoint.Code, invoice.Sequential);
                var unsignedXml = invoiceXmlBuilder.BuildXMLInvoice(invoice, invoice.Business, invoice.Establishment, invoice.EmissionPoint, invoice.Customer);

                var (pfxBytes, pfxPassword) = await GetCertificateAsync(businessId);


                var signedXml = await electronicSignature.SignXmlAsync(
                    unsignedXml,
                    pfxBytes,
                    pfxPassword,
                    cancellationToken: default
                );

                invoice.XmlSigned = signedXml;
                invoice.Status = InvoiceStatuses.SIGNED;

                var receptionResponse = await sriReceptionService.SendInvoiceSriAsync(signedXml, invoice.Environment == "2");

                if (receptionResponse.State == InvoiceStatuses.SRI_RECEIVED)
                {
                    invoice.Status = InvoiceStatuses.SRI_RECEIVED;
                    invoice.SriMessage = receptionResponse.Message;
                }
                else if (receptionResponse.State == InvoiceStatuses.SRI_REJECTED)
                {
                    invoice.Status = InvoiceStatuses.SRI_REJECTED;
                    invoice.SriMessage = receptionResponse.Message;
                }
                else if (receptionResponse.State == InvoiceStatuses.SRI_RETURNED)
                {
                    invoice.Status = InvoiceStatuses.SRI_RETURNED;
                    invoice.SriMessage = receptionResponse.Message;
                }
                else if (receptionResponse.State is InvoiceStatuses.SRI_TIMEOUT or InvoiceStatuses.SRI_UNAVAILABLE)
                {
                    invoice.Status = InvoiceStatuses.SRI_UNAVAILABLE;
                }
                else
                {
                    invoice.Status = InvoiceStatuses.ERROR;
                }
            }
            else
            {
                invoice.AccessKey = string.Empty;
                invoice.XmlSigned = null;
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            response.Data = new InvoiceComplexResDto
            {
                Id = invoice.Id,
                Sequential = invoice.Sequential,
                AccessKey = invoice.AccessKey,
                AuthorizationNumber = invoice.AuthorizationNumber,
                Environment = invoice.Environment,
                DocumentType = invoice.DocumentType,
                Status = invoice.Status,
                IsElectronic = invoice.IsElectronic,
                InvoiceDate = invoice.InvoiceDate,
                DueDate = invoice.DueDate,
                BusinessId = invoice.Business.Id,
                BusinessDocument = invoice.Business.Document,
                BusinessName = invoice.Business.Name,
                EstablishmentId = invoice.Establishment.Id,
                EstablishmentCode = invoice.Establishment.Code,
                EstablishmentName = invoice.Establishment.Name,
                EmissionPointId = invoice.EmissionPoint.Id,
                EmissionPointCode = invoice.EmissionPoint.Code,
                EmissionPointDescription = invoice.EmissionPoint.Description,
                UserId = invoice.User!.Id,
                UserDocument = invoice.User.Document,
                UserFullName = invoice.User.FullName,
                Customer = new CustomerResDto
                {
                    Id = invoice.Customer.Id,
                    DocumentType = invoice.Customer.DocumentType,
                    Document = invoice.Customer.Document,
                    Email = invoice.Customer.Email,
                    Name = invoice.Customer.Name,
                    Cellphone = invoice.Customer.Cellphone,
                    Telephone = invoice.Customer.Telephone,
                    Address = invoice.Customer.Address,
                    IsActive = invoice.Customer.IsActive,
                    CreatedAt = invoice.Customer.CreatedAt
                },
                SubtotalWithoutTaxes = invoice.SubtotalWithoutTaxes,
                SubtotalWithTaxes = invoice.SubtotalWithTaxes,
                DiscountTotal = invoice.DiscountTotal,
                TaxTotal = invoice.TaxTotal,
                TotalInvoice = invoice.TotalInvoice,
                PaymentMethod = invoice.PaymentMethod,
                PaymentTermDays = invoice.PaymentTermDays,
                Description = invoice.Description ?? "",
                AdditionalInformation = invoice.AdditionalInformation,
                AuthorizationDate = invoice.AuthorizationDate,
                SriMessage = invoice.SriMessage,
                XmlSigned = invoice.XmlSigned ?? "",
                Details = [.. invoice.InvoiceDetails.Select(d => new InvoiceDetailResDto
                {
                    Id = d.Id,
                    InvoiceId = d.InvoiceId,
                    ProductId = d.Product!.Id,
                    ProductCode = d.Product.Sku,
                    ProductName = d.Product.Name,
                    UnitMeasureId = d.Product.UnitMeasureId,
                    UnitMeasureCode = d.Product.UnitMeasure!.Code,
                    UnitMeasureName = d.Product.UnitMeasure!.Name,
                    WarehouseId = d.Warehouse!.Id,
                    WarehouseCode = d.Warehouse.Code,
                    WarehouseName = d.Warehouse.Name,
                    TaxId = d.Tax!.Id,
                    TaxCode = d.Tax.Code,
                    TaxName = d.Tax.Name,
                    TaxRate = d.TaxRate,
                    TaxValue = d.TaxValue,
                    Quantity = d.Quantity,
                    UnitPrice = d.UnitPrice,
                    Discount = d.Discount,
                    Subtotal = d.Subtotal,
                    Total = d.Total
                })]
            };

            response.Success = true;
            response.Message = invoice.SriMessage ?? "Factura actualizada correctamente.";

            return response;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            response.Success = false;
            response.Message = "Error al actualizar la factura";
            response.Error = ex.Message;
        }

        return response;
    }

    private static string GenerateAccessKey(DateTime date, string documentType, string businessDocument, string environment, string establishment, string emissionPoint, string sequencial)
    {
        string currentDate = date.ToString("ddMMyyyy");

        string docType = documentType.PadLeft(2, '0');

        string document = businessDocument.PadLeft(13, '0');

        string invoiceEnvironment = environment;

        string serie = $"{establishment}{emissionPoint}";

        string sec = sequencial.PadLeft(9, '0');

        string numericCode = new Random().Next(10000000, 99999999).ToString();

        string emissionType = "1";

        string preKey = $"{currentDate}{docType}{document}{invoiceEnvironment}{serie}{sec}{numericCode}{emissionType}";

        string dv = CalculateCheckDigit(preKey).ToString();

        return preKey + dv;
    }

    private static int CalculateCheckDigit(string chain)
    {
        int[] factors = [2, 3, 4, 5, 6, 7];
        int factorIndex = 0;
        int sum = 0;

        for (int i = chain.Length - 1; i >= 0; i--)
        {
            int digit = int.Parse(chain[i].ToString());
            sum += digit * factors[factorIndex];
            factorIndex = (factorIndex + 1) % factors.Length;
        }

        int modulo = sum % 11;
        int digitVerifier = 11 - modulo;

        if (digitVerifier == 10) digitVerifier = 1;
        if (digitVerifier == 11) digitVerifier = 0;

        return digitVerifier;
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

    private async Task<(byte[] pfxBytes, string password)> GetCertificateAsync(int businessId)
    {
        var cert = await context.BusinessCertificates
            .FirstOrDefaultAsync(c => c.BusinessId == businessId)
            ?? throw new Exception("El negocio no tiene certificado cargado");

        var bytes = Convert.FromBase64String(cert.CertificateBase64);
        var pass = aes.Decrypt(cert.Password);

        return (bytes, pass);
    }
}
