using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Core.Constants;
using Core.DTOs;
using Core.DTOs.InvoiceDto;
using Core.Interfaces.Repository;
using Core.Interfaces.Services.IUtilService;
using Core.Interfaces.Services.IInvoiceService;
using Core.Interfaces.Specifications.InvoiceSpecification;
using Infrastructure.Specification.InvoiceSpecification;

namespace Infrastructure.Data;

public class InvoiceRepository(
    StoreContext context,
    IUserContextService currentUser,
    IInvoiceSequentialService seq,
    IInvoiceStockService stock,
    IInvoiceValidationService validate,
    IInvoiceCalculationService calc,
    IInvoiceDtoFactory invoiceDtoFactory) : IInvoiceRepository
{
    public async Task<ApiResponse<InvoiceComplexResDto>> ConfirmInvoiceAsync(int id)
    {
        var response = new ApiResponse<InvoiceComplexResDto>();

        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
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
            .FirstOrDefaultAsync(i => i.BusinessId == currentUser.BusinessId && i.Id == id);

            if (existingInvoice == null)
            {
                response.Success = false;
                response.Message = "Factura no encontrada";
                response.Error = "No existe una factura con el identificador proporcionado";
                return response;
            }

            if (existingInvoice.Status != InvoiceStatus.DRAFT)
            {
                response.Success = false;
                response.Message = "La factura no está en estado borrador";
                response.Error = "Solo se pueden confirmar facturas en estado borrador";
                return response;
            }

            ValidateRequiredSriFields(existingInvoice);

            existingInvoice.Status = InvoiceStatus.PENDING;

            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            response.Success = true;
            response.Message = "Factura lista para confirmación";
            response.Data = invoiceDtoFactory.InvoiceComplexRes(existingInvoice);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al preparar la factura para confirmación";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<InvoiceSimpleResDto>> CreateInvoiceAsync(InvoiceCreateReqDto invoiceCreateReqDto)
    {
        var response = new ApiResponse<InvoiceSimpleResDto>();

        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            if (currentUser.BusinessId == 0 ||
            currentUser.EstablishmentId == 0 ||
            currentUser.EmissionPointId == 0 ||
            currentUser.UserId == 0)
            {
                response.Success = false;
                response.Message = "Datos de autenticación incompletos";
                response.Error = "No se pudo validar el negocio, establecimiento, punto de emisión o usuario";

                return response;
            }

            var customer = await validate.ValidateCustomerAsync(invoiceCreateReqDto.CustomerId);
            var business = await validate.ValidateBusinessAsync(currentUser.BusinessId);
            var establishment = await validate.ValidateEstablishmentAsync(currentUser.EstablishmentId);
            var emissionPoint = await validate.ValidateEmissionPointAsync(currentUser.EmissionPointId);
            var user = await validate.ValidateUserAsync(currentUser.UserId);

            var sequence = await seq.GetNextSequentialAsync(
                business.Id,
                establishment.Id,
                emissionPoint.Id);

            var ecTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow,
              TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil"));

            var newInvoice = new Invoice
            {
                ReceiptType = invoiceCreateReqDto.ReceiptType,
                Environment = invoiceCreateReqDto.Environment,
                IsElectronic = invoiceCreateReqDto.IsElectronic,
                InvoiceDate = ecTime,
                DueDate = ecTime,
                CustomerId = customer.Id,
                BusinessId = business.Id,
                EstablishmentId = establishment.Id,
                EmissionPointId = emissionPoint.Id,
                UserId = user.Id,
                Sequential = sequence,
                AccessKey = "",
                PaymentMethod = invoiceCreateReqDto.PaymentMethod,
                PaymentTermDays = invoiceCreateReqDto.PaymentTermDays,
                Description = invoiceCreateReqDto.Description,
                AdditionalInformation = invoiceCreateReqDto.AdditionalInformation,
                Status = InvoiceStatus.DRAFT,
                Business = business,
                Establishment = establishment,
                EmissionPoint = emissionPoint,
                Customer = customer
            };

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

                    continue;
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

                    continue;
                }

                await stock.ReserveStockAsync(detail.ProductId, detail.WarehouseId, detail.Quantity);

                var subtotal = detail.Quantity * product.Price;
                var discount = detail.Discount;
                var taxableBase = subtotal - discount;

                var taxRate = product.Tax?.Rate ?? 0;
                var taxValue = taxableBase * (taxRate / 100);
                var total = taxableBase + taxValue;

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

            var totals = calc.Calculate(newInvoice);

            newInvoice.SubtotalWithoutTaxes = totals.SubtotalWithTaxes;
            newInvoice.SubtotalWithTaxes = totals.SubtotalWithTaxes;
            newInvoice.DiscountTotal = totals.DiscountTotal;
            newInvoice.TaxTotal = totals.TaxTotal;
            newInvoice.TotalInvoice = totals.SubtotalWithTaxes;
            newInvoice.AccessKey = string.Empty;
            newInvoice.XmlSigned = null;

            context.Invoices.Add(newInvoice);
            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            response.Success = true;
            response.Message = "Factura creada exitosamente.";
            response.Data = invoiceDtoFactory.InvoiceSimpleRes(newInvoice);

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
            if (currentUser.BusinessId == 0)
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
            .FirstOrDefaultAsync(i => i.BusinessId == currentUser.BusinessId && i.Id == id);

            if (existingInvoice == null)
            {
                response.Success = false;
                response.Message = "Factura no encontrada";
                response.Error = "No existe una Factura con el ID especificado";

                return response;
            }

            response.Success = true;
            response.Message = "Factura obtenida correctamente";
            response.Data = invoiceDtoFactory.InvoiceComplexRes(existingInvoice);

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
            if (currentUser.BusinessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no encontrado";
                response.Error = "No existe un negocio con el ID especificado";
                return response;
            }

            var query = context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Business)
            .Include(i => i.Establishment)
            .Include(i => i.EmissionPoint)
            .Include(i => i.User)
            .Where(
                i =>
                i.BusinessId == currentUser.BusinessId &&
                i.EmissionPointId == currentUser.EmissionPointId &&
                i.EstablishmentId == currentUser.EstablishmentId &&
                i.UserId == currentUser.UserId);

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

            var invoiceList = await query
                .OrderByDescending(i => i.InvoiceDate)
                .Skip(skip)
                .Take(limit)
                .ToListAsync();

            var invoices = invoiceList.Select(i => invoiceDtoFactory.InvoiceSimpleRes(i)).ToList();

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
            if (currentUser.BusinessId == 0 ||
            currentUser.EstablishmentId == 0 ||
            currentUser.EmissionPointId == 0 ||
            currentUser.UserId == 0)
            {
                response.Success = false;
                response.Message = "Datos de autenticación incompletos";
                response.Error = "No se pudo validar el negocio, establecimiento, punto de emisión o usuario";

                return response;
            }

            var existingInvoice = await context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.Business)
            .Include(i => i.Establishment)
            .Include(i => i.EmissionPoint)
            .Include(i => i.User)
            .Include(i => i.InvoiceDetails)
            .ThenInclude(d => d.Product).ThenInclude(p => p!.UnitMeasure)
            .Include(i => i.InvoiceDetails)
            .ThenInclude(d => d.Warehouse)
            .Include(i => i.InvoiceDetails)
            .ThenInclude(d => d.Tax)
            .FirstOrDefaultAsync(
                i =>
                i.Id == invoiceId &&
                i.BusinessId == currentUser.BusinessId &&
                i.EstablishmentId == currentUser.EstablishmentId &&
                i.EmissionPointId == currentUser.EmissionPointId &&
                i.UserId == currentUser.UserId);

            if (existingInvoice == null)
            {
                response.Success = false;
                response.Message = "Factura no encontrada";
                response.Error = "No existe una factura con el ID especificado";

                return response;
            }

            var ecTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil"));

            existingInvoice.Environment = invoiceUpdateReqDto.Environment;
            existingInvoice.InvoiceDate = ecTime;
            existingInvoice.DueDate = ecTime;
            existingInvoice.PaymentTermDays = invoiceUpdateReqDto.PaymentTermDays;
            existingInvoice.Description = invoiceUpdateReqDto.Description;
            existingInvoice.AdditionalInformation = invoiceUpdateReqDto.AdditionalInformation;

            var incomingKeys = invoiceUpdateReqDto.Details
            .Select(d => (d.ProductId, d.WarehouseId))
            .ToHashSet();

            foreach (var oldDetail in existingInvoice.InvoiceDetails.ToList())
            {
                var key = (oldDetail.ProductId, oldDetail.WarehouseId);

                if (!incomingKeys.Contains(key))
                {
                    await stock.ReturnStockAsync(oldDetail.ProductId, oldDetail.WarehouseId, oldDetail.Quantity);

                    context.InvoiceDetails.Remove(oldDetail);
                    existingInvoice.InvoiceDetails.Remove(oldDetail);
                }
            }

            foreach (var dtoDetail in invoiceUpdateReqDto.Details)
            {
                var existingDetail = existingInvoice.InvoiceDetails
                .FirstOrDefault(
                    d =>
                    d.ProductId == dtoDetail.ProductId &&
                    d.WarehouseId == dtoDetail.WarehouseId);

                if (existingDetail != null)
                {
                    var product = await context.Products
                    .Include(p => p.Tax)
                    .FirstOrDefaultAsync(p => p.Id == dtoDetail.ProductId) ??
                    throw new Exception($"Producto {dtoDetail.ProductId} no encontrado");

                    var diff = dtoDetail.Quantity - existingDetail.Quantity;

                    if (diff > 0)
                    {
                        await stock.ReserveStockAsync(dtoDetail.ProductId, dtoDetail.WarehouseId, diff);
                    }
                    else if (diff < 0)
                    {
                        await stock.ReturnStockAsync(dtoDetail.ProductId, dtoDetail.WarehouseId, -diff);
                    }

                    existingDetail.Quantity = dtoDetail.Quantity;
                    existingDetail.Discount = dtoDetail.Discount;

                    var subtotal = existingDetail.Quantity * product.Price;
                    var taxableBase = subtotal - existingDetail.Discount;
                    var taxRate = product.Tax?.Rate ?? 0;
                    var taxValue = taxableBase * (taxRate / 100);
                    var total = taxableBase + taxValue;

                    existingDetail.UnitPrice = product.Price;
                    existingDetail.Subtotal = taxableBase;
                    existingDetail.TaxValue = taxValue;
                    existingDetail.Total = total;
                }
                else
                {
                    var product = await context.Products
                    .Include(p => p.Tax)
                    .FirstOrDefaultAsync(p => p.Id == dtoDetail.ProductId);

                    var warehouse = await context.Warehouses
                    .FirstOrDefaultAsync(w => w.Id == dtoDetail.WarehouseId);

                    var stockRecord = await context.ProductWarehouses
                    .FirstOrDefaultAsync(
                        pw =>
                        pw.ProductId == dtoDetail.ProductId &&
                        pw.WarehouseId == dtoDetail.WarehouseId);

                    if (product == null)
                    {
                        response.Success = false;
                        response.Message = "Producto no encontrado";
                        response.Error = $"Producto {dtoDetail.ProductId} no encontrado";

                        continue;
                    }

                    if (stockRecord == null || stockRecord.Stock < dtoDetail.Quantity)
                    {
                        response.Success = false;
                        response.Message = "Stock insuficiente";
                        response.Error = $"Stock insuficiente del producto {product.Name} en bodega {warehouse!.Code}";

                        continue;
                    }

                    await stock.ReserveStockAsync(dtoDetail.ProductId, dtoDetail.WarehouseId, dtoDetail.Quantity);

                    var subtotal = dtoDetail.Quantity * product.Price;
                    var taxableBase = subtotal - dtoDetail.Discount;
                    var taxRate = product.Tax?.Rate ?? 0;
                    var taxValue = taxableBase * (taxRate / 100);
                    var total = taxableBase + taxValue;

                    var newDetail = new InvoiceDetail
                    {
                        InvoiceId = existingInvoice.Id,
                        ProductId = dtoDetail.ProductId,
                        WarehouseId = dtoDetail.WarehouseId,
                        Quantity = dtoDetail.Quantity,
                        UnitPrice = product.Price,
                        Discount = dtoDetail.Discount,
                        Subtotal = taxableBase,
                        TaxId = product.TaxId,
                        TaxRate = taxRate,
                        TaxValue = taxValue,
                        Total = total,
                        Product = product,
                        Warehouse = warehouse,
                        Tax = product.Tax
                    };

                    existingInvoice.InvoiceDetails.Add(newDetail);
                }
            }

            var totals = calc.Calculate(existingInvoice);

            existingInvoice.SubtotalWithoutTaxes = totals.SubtotalWithoutTaxes;
            existingInvoice.SubtotalWithTaxes = totals.SubtotalWithTaxes;
            existingInvoice.DiscountTotal = totals.DiscountTotal;
            existingInvoice.TaxTotal = totals.TaxTotal;
            existingInvoice.TotalInvoice = totals.SubtotalWithTaxes;
            existingInvoice.Status = InvoiceStatus.DRAFT;

            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            response.Success = true;
            response.Data = invoiceDtoFactory.InvoiceComplexRes(existingInvoice);
            response.Message = "Factura actualizada correctamente";

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

    public async Task<ApiResponse<InvoiceComplexResDto>> UpdateInvoicePaymentAsync(int invoiceId, InvoicePaymentUpdateReqDto invoicePaymentUpdateReqDto)
    {
        var response = new ApiResponse<InvoiceComplexResDto>();

        try
        {
            if (currentUser.BusinessId == 0 ||
             currentUser.EstablishmentId == 0 ||
             currentUser.EmissionPointId == 0 ||
             currentUser.UserId == 0)
            {
                response.Success = false;
                response.Message = "Datos de autenticación incompletos";
                response.Error = "No se pudo validar el negocio, establecimiento, punto de emisión o usuario";

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
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Warehouse)
                .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Tax)
                .FirstOrDefaultAsync(
                    i =>
                    i.Id == invoiceId &&
                    i.BusinessId == currentUser.BusinessId &&
                    i.EstablishmentId == currentUser.EstablishmentId &&
                    i.EmissionPointId == currentUser.EmissionPointId &&
                    i.UserId == currentUser.UserId);

            if (existingInvoice == null)
            {
                response.Success = false;
                response.Message = "Factura no encontrada";
                response.Error = "No existe una factura con el ID especificado";

                return response;
            }

            if (existingInvoice.InvoiceDetails.Count == 0)
            {
                response.Success = false;
                response.Message = "La factura no tiene detalles";
                response.Error = "No se puede registrar un pago en una factura sin productos";

                return response;
            }

            validate.ValidateInvoiceIsDraft(existingInvoice);

            if (invoicePaymentUpdateReqDto.PaymentTermDays < 0)
            {
                response.Success = false;
                response.Message = "Plazo de pago inválido";
                response.Error = "Los días de pago no pueden ser negativos";

                return response;
            }

            if (string.IsNullOrWhiteSpace(invoicePaymentUpdateReqDto.PaymentMethod))
            {
                response.Success = false;
                response.Message = "Método de pago requerido";
                response.Error = "Debe especificar un método de pago";

                return response;
            }


            if (existingInvoice.TotalInvoice <= 0)
            {
                response.Success = false;
                response.Message = "Total en 0 o negativo";
                response.Error = "El total enviado no puede ser 0 o negativo";

                return response;
            }

            existingInvoice.PaymentMethod = invoicePaymentUpdateReqDto.PaymentMethod;
            existingInvoice.PaymentTermDays = invoicePaymentUpdateReqDto.PaymentTermDays;

            await context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Pago registrado en factura";
            response.Data = invoiceDtoFactory.InvoiceComplexRes(existingInvoice);

        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al actualizar el pago de la factura";
            response.Error = ex.Message;

            return response;
        }

        return response;
    }

    private static void ValidateRequiredSriFields(Core.Entities.Invoice invoice)
    {
        if (invoice.Business is null || string.IsNullOrWhiteSpace(invoice.Business.Document))
        {
            throw new Exception("El negocio no tiene RUC configurado");
        }

        if (invoice.Establishment is null || string.IsNullOrWhiteSpace(invoice.Establishment.Code))
        {
            throw new Exception("El establecimiento no tiene código configurado");
        }

        if (invoice.EmissionPoint is null || string.IsNullOrWhiteSpace(invoice.EmissionPoint.Code))
        {
            throw new Exception("El punto de emisión no tiene código configurado");
        }

        if (invoice.Customer is null || string.IsNullOrWhiteSpace(invoice.Customer.Document))
        {
            throw new Exception("El cliente no tiene documento configurado");
        }

        if (string.IsNullOrWhiteSpace(invoice.Sequential))
        {
            throw new Exception("La factura no tiene secuencial configurado");
        }

        if (invoice.InvoiceDetails.Count == 0)
        {
            throw new Exception("La factura no tiene detalles asociados");
        }
    }
}
