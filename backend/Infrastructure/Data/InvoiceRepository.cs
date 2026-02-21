using Microsoft.EntityFrameworkCore;
using Core.Constants;
using Core.DTOs;
using Core.DTOs.InvoiceDto;
using Core.Interfaces.Repository;
using Core.Interfaces.Services.IUtilService;
using Core.Interfaces.Services.IInvoiceService;
using Core.Interfaces.Services.IKardexService;
using Core.Interfaces.Services.IARService;
using Core.DTOs.ARDto;
using Core.Entities;

namespace Infrastructure.Data;

public class InvoiceRepository(
    StoreContext context,
    IUserContextService currentUser,
    IInvoiceSequentialService seq,
    IKardexService kardex,
    IInvoiceEditionService edition,
    IInvoiceValidationService validate,
    IInvoiceCalculationService calc,
    IAccountsReceivableService accountsReceivableService,
    IInvoiceDtoFactory invoiceDtoFactory) : IInvoiceRepository
{
    public async Task<ApiResponse<InvoiceComplexResDto>> ConfirmInvoiceAsync(int id)
    {
        var response = new ApiResponse<InvoiceComplexResDto>();

        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var existingInvoice = await FindInvoiceAsync(id);

            if (existingInvoice == null)
            {
                response.Success = false;
                response.Message = "Factura no encontrada";
                response.Error = "No existe una factura con el ID proporcionado";

                return response;
            }

            if (existingInvoice.Status != InvoiceStatus.DRAFT)
            {
                response.Success = false;
                response.Message = "La factura no está en estado borrador";
                response.Error = "Solo se pueden confirmar facturas en estado borrador";

                return response;
            }

            await kardex.DecreaseStockForSaleAsync(existingInvoice);

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
            ValidateCurrentUser();

            var customer = await validate.ValidateCustomerAsync(invoiceCreateReqDto.CustomerId);
            var business = await validate.ValidateBusinessAsync(currentUser.BusinessId);
            var establishment = await validate.ValidateEstablishmentAsync(currentUser.EstablishmentId);
            var emissionPoint = await validate.ValidateEmissionPointAsync(currentUser.EmissionPointId);
            var user = await validate.ValidateUserAsync(currentUser.UserId);

            var sequence = await seq.GetNextSequentialAsync(business.Id, establishment.Id, emissionPoint.Id);

            var ecTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow,
              TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil"));

            var newInvoice = edition.BuildInvoice(
                invoiceCreateReqDto,
                customer,
                business,
                establishment,
                emissionPoint,
                user,
                sequence,
                ecTime);

            await edition.AddInvoiceDetailsAsync(newInvoice, invoiceCreateReqDto.Details);

            var totals = calc.Calculate(newInvoice);

            newInvoice.SubtotalWithoutTaxes = totals.SubtotalWithoutTaxes;
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

            var existingInvoice = await FindInvoiceAsync(id);

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
            .Where(i => i.BusinessId == currentUser.BusinessId);

            if (!currentUser.IsAdmin)
            {
                query = query.Where(i =>
                    i.EmissionPointId == currentUser.EmissionPointId &&
                    i.EstablishmentId == currentUser.EstablishmentId &&
                    i.UserId == currentUser.UserId);
            }

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

            var invoices = invoiceList.Select(invoiceDtoFactory.InvoiceSimpleRes).ToList();

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
            ValidateCurrentUser();

            var existingInvoice = await FindInvoiceAsync(invoiceId);

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

            await edition.UpsertInvoiceAsync(existingInvoice, invoiceUpdateReqDto, invoiceUpdateReqDto.Details);

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

        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            ValidateCurrentUser();

            var existingInvoice = await FindInvoiceAsync(invoiceId);

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

            if (!TryNormalizePaymentType(invoicePaymentUpdateReqDto.PaymentType, out var normalizedPaymentType))
            {
                response.Success = false;
                response.Message = "Tipo de pago requerido";
                response.Error = "Debe especificar un tipo de pago válido";

                return response;
            }

            await kardex.DecreaseStockForSaleAsync(existingInvoice);

            if (existingInvoice.TotalInvoice <= 0)
            {
                response.Success = false;
                response.Message = "Total en 0 o negativo";
                response.Error = "El total enviado no puede ser 0 o negativo";

                return response;
            }

            var requiresAR = invoicePaymentUpdateReqDto.PaymentTermDays > 0 ||
            !invoicePaymentUpdateReqDto.PaymentMethod.Equals(
                PaymentMethod.NFS,
                StringComparison.OrdinalIgnoreCase);

            existingInvoice.PaymentMethod = invoicePaymentUpdateReqDto.PaymentMethod;
            existingInvoice.PaymentType = normalizedPaymentType;
            existingInvoice.PaymentTermDays = invoicePaymentUpdateReqDto.PaymentTermDays;
            existingInvoice.Status = InvoiceStatus.PENDING;

            if (requiresAR)
            {
                var arDto = new ARCreateFromInvoiceReqDto
                {
                    PaymentMethod = invoicePaymentUpdateReqDto.PaymentMethod,
                    TermDays = invoicePaymentUpdateReqDto.PaymentTermDays,
                    ExpectedPaymentDate = invoicePaymentUpdateReqDto.ExpectedPaymentDate,
                    Reference = invoicePaymentUpdateReqDto.Reference,
                    Notes = invoicePaymentUpdateReqDto.Notes,
                    InitialPaymentAmount = 0,
                    InitialPaymentMethodCode = null
                };

                await accountsReceivableService.UpsertFromInvoiceAsync(existingInvoice, arDto);
            }

            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            response.Success = true;
            response.Message = "Pago registrado en factura";
            response.Data = invoiceDtoFactory.InvoiceComplexRes(existingInvoice);

        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            response.Success = false;
            response.Message = "Error al actualizar el pago de la factura";
            response.Error = ex.Message;

            return response;
        }

        return response;
    }

    private async Task<Invoice?> FindInvoiceAsync(int invoiceId)
    {
        if (currentUser.IsAdmin)
            return await edition.CheckInvoiceExistenceAsync(invoiceId, currentUser.BusinessId);

        return await edition.CheckInvoiceExistenceAsync(
            invoiceId,
            currentUser.BusinessId,
            currentUser.EstablishmentId,
            currentUser.EmissionPointId,
            currentUser.UserId);
    }

    private void ValidateCurrentUser()
    {
        if (currentUser.BusinessId == 0 ||
         currentUser.EstablishmentId == 0 ||
         currentUser.EmissionPointId == 0 ||
         currentUser.UserId == 0)
        {
            throw new InvalidOperationException("Datos de autenticación incompletos");
        }
    }

    private static bool TryNormalizePaymentType(string? paymentType, out string normalizedPaymentType)
    {
        normalizedPaymentType = string.Empty;

        if (string.IsNullOrWhiteSpace(paymentType))
        {
            return false;
        }

        var trimmed = paymentType.Trim();

        if (trimmed.Equals(PaymentType.CASH, StringComparison.OrdinalIgnoreCase))
        {
            normalizedPaymentType = PaymentType.CASH;
            return true;
        }

        if (trimmed.Equals(PaymentType.CHECK, StringComparison.OrdinalIgnoreCase))
        {
            normalizedPaymentType = PaymentType.CHECK;
            return true;
        }

        if (trimmed.Equals(PaymentType.CREDIT_CARD, StringComparison.OrdinalIgnoreCase))
        {
            normalizedPaymentType = PaymentType.CREDIT_CARD;
            return true;
        }

        if (trimmed.Equals(PaymentType.DEBIT_CARD, StringComparison.OrdinalIgnoreCase))
        {
            normalizedPaymentType = PaymentType.DEBIT_CARD;
            return true;
        }

        return false;
    }
}
