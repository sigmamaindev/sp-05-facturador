using Core.DTOs;
using Core.DTOs.AtsDto;
using Core.Entities;
using Core.Interfaces.Repository;
using Core.Interfaces.Services.IAtsService;
using Core.Interfaces.Services.IUtilService;
using Core.Constants;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AtsRepository(
    StoreContext context,
    IUserContextService currentUser,
    IAtsXmlBuilderService xmlBuilder) : IAtsRepository
{
    public async Task<ApiResponse<List<AtsPurchaseResDto>>> GetAtsPurchasesAsync(int year, int month)
    {
        var response = new ApiResponse<List<AtsPurchaseResDto>>();

        try
        {
            ValidateCurrentUser();
            ValidatePeriod(year, month);

            var purchases = await LoadPurchasesAsync(year, month);

            response.Success = true;
            response.Message = "Valores ATS de compras obtenidos correctamente";
            response.Data = [.. purchases.Select(MapToAtsPurchase)];
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener valores ATS de compras";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<List<AtsSaleResDto>>> GetAtsSalesAsync(int year, int month)
    {
        var response = new ApiResponse<List<AtsSaleResDto>>();

        try
        {
            ValidateCurrentUser();
            ValidatePeriod(year, month);

            var invoices = await LoadInvoicesAsync(year, month);
            var sales = GroupInvoicesToAtsSales(invoices);

            response.Success = true;
            response.Message = "Valores ATS de ventas obtenidos correctamente";
            response.Data = sales;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener valores ATS de ventas";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<string>> GetAtsXmlAsync(int year, int month)
    {
        var response = new ApiResponse<string>();

        try
        {
            ValidateCurrentUser();
            ValidatePeriod(year, month);

            var business = await context.Businesses
                .FirstOrDefaultAsync(b => b.Id == currentUser.BusinessId)
                ?? throw new InvalidOperationException("Negocio no encontrado");

            var establishment = await context.Establishments
                .FirstOrDefaultAsync(e => e.Id == currentUser.EstablishmentId)
                ?? throw new InvalidOperationException("Establecimiento no encontrado");

            var purchases = await LoadPurchasesAsync(year, month);
            var atsPurchases = purchases.Select(MapToAtsPurchase).ToList();

            var invoices = await LoadInvoicesAsync(year, month);
            var atsSales = GroupInvoicesToAtsSales(invoices);

            var totalVentas = atsSales.Sum(s => s.Total);

            response.Success = true;
            response.Message = "XML ATS generado correctamente";
            response.Data = xmlBuilder.BuildAtsXml(year, month, business, establishment.Code,
                atsPurchases, atsSales, totalVentas);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al generar el XML ATS";
            response.Error = ex.Message;
        }

        return response;
    }

    private async Task<List<Purchase>> LoadPurchasesAsync(int year, int month)
    {
        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1);

        return await context.Purchases
            .Include(p => p.PurchaseDetails)
                .ThenInclude(d => d.Tax)
            .Where(p =>
                p.BusinessId == currentUser.BusinessId &&
                p.IssueDate >= start &&
                p.IssueDate < end)
            .OrderBy(p => p.IssueDate)
            .ThenBy(p => p.Id)
            .ToListAsync();
    }

    private async Task<List<Invoice>> LoadInvoicesAsync(int year, int month)
    {
        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1);

        return await context.Invoices
            .Include(i => i.Customer)
            .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Tax)
            .Where(i =>
                i.BusinessId == currentUser.BusinessId &&
                i.InvoiceDate >= start &&
                i.InvoiceDate < end &&
                i.Status != InvoiceStatus.DRAFT)
            .OrderBy(i => i.InvoiceDate)
            .ThenBy(i => i.Id)
            .ToListAsync();
    }

    private static AtsPurchaseResDto MapToAtsPurchase(Purchase purchase)
    {
        var details = purchase.PurchaseDetails ?? [];

        var baseImponible = details
            .Where(d => ResolveTaxRate(d) == 0)
            .Sum(d => d.Subtotal);

        var baseImpGrav = details
            .Where(d => ResolveTaxRate(d) > 0)
            .Sum(d => d.Subtotal);

        var montoIva = details
            .Where(d => ResolveTaxRate(d) > 0)
            .Sum(d => d.TaxValue);

        var idProv = !string.IsNullOrWhiteSpace(purchase.DocumentSubjectDetained)
            ? purchase.DocumentSubjectDetained!
            : purchase.Document;

        var autorizacion = !string.IsNullOrWhiteSpace(purchase.AuthorizationNumber)
            ? purchase.AuthorizationNumber!
            : purchase.AccessKey;

        return new AtsPurchaseResDto
        {
            PurchaseId = purchase.Id,
            CodSustento = purchase.SupportingCode ?? string.Empty,
            TpIdProv = purchase.TypeDocumentSubjectDetained,
            IdProv = idProv,
            TipoComprobante = purchase.SupportingDocumentCode ?? string.Empty,
            ParteRel = purchase.RelatedParty,
            FechaRegistro = purchase.IssueDate,
            Establecimiento = purchase.EstablishmentCode,
            PuntoEmision = purchase.EmissionPointCode,
            Secuencial = purchase.Sequential,
            FechaEmision = purchase.IssueDate,
            Autorizacion = autorizacion,
            BaseNoGraIva = 0m,
            BaseImponible = baseImponible,
            BaseImpGrav = baseImpGrav,
            BaseImpExe = 0m,
            MontoIce = 0m,
            MontoIva = montoIva,
            ValRetBien10 = 0m,
            ValRetServ20 = 0m,
            ValorRetBienes = 0m,
            ValRetServ50 = 0m,
            ValorRetServicios = 0m,
            ValRetServ100 = 0m,
            TotbasesImpReemb = 0m,
            Total = purchase.TotalPurchase,
            ProveedorRazonSocial = purchase.BusinessName
        };
    }

    private static List<AtsSaleResDto> GroupInvoicesToAtsSales(IEnumerable<Invoice> invoices)
    {
        return [.. invoices
            .Where(i => i.Customer != null)
            .GroupBy(i => new
            {
                TpIdCliente = i.Customer!.DocumentType,
                IdCliente = i.Customer.Document,
                TipoComprobante = i.ReceiptType,
                TipoEmision = i.IsElectronic ? "E" : "F",
                ParteRelVtas = "NO",
                ClienteRazonSocial = i.Customer.Name,
                PaymentMethod = i.PaymentMethod
            })
            .Select(group =>
            {
                var baseImponible = group
                    .SelectMany(i => i.InvoiceDetails ?? [])
                    .Where(d => ResolveTaxRate(d) == 0)
                    .Sum(d => d.Subtotal);

                var baseImpGrav = group
                    .SelectMany(i => i.InvoiceDetails ?? [])
                    .Where(d => ResolveTaxRate(d) > 0)
                    .Sum(d => d.Subtotal);

                var montoIva = group
                    .SelectMany(i => i.InvoiceDetails ?? [])
                    .Where(d => ResolveTaxRate(d) > 0)
                    .Sum(d => d.TaxValue);

                var total = group.Sum(i => i.TotalInvoice);

                return new AtsSaleResDto
                {
                    TpIdCliente = group.Key.TpIdCliente,
                    IdCliente = group.Key.IdCliente,
                    ParteRelVtas = group.Key.ParteRelVtas,
                    TipoComprobante = group.Key.TipoComprobante,
                    TipoEmision = group.Key.TipoEmision,
                    NumeroComprobantes = group.Count(),
                    BaseNoGraIva = 0m,
                    BaseImponible = baseImponible,
                    BaseImpGrav = baseImpGrav,
                    MontoIce = 0m,
                    MontoIva = montoIva,
                    ValorRetIva = 0m,
                    ValorRetRenta = 0m,
                    FormaPago = group.Key.PaymentMethod,
                    Total = total,
                    ClienteRazonSocial = group.Key.ClienteRazonSocial
                };
            })
            .OrderBy(x => x.TpIdCliente)
            .ThenBy(x => x.IdCliente)
            .ThenBy(x => x.TipoComprobante)];
    }

    private static decimal ResolveTaxRate(PurchaseDetail detail)
        => detail.Tax?.Rate ?? detail.TaxRate;

    private static decimal ResolveTaxRate(InvoiceDetail detail)
        => detail.Tax?.Rate ?? detail.TaxRate;

    private static void ValidatePeriod(int year, int month)
    {
        if (year < 1900)
        {
            throw new ArgumentOutOfRangeException(nameof(year), "El año debe ser mayor o igual a 1900");
        }

        if (month is < 1 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(month), "El mes debe estar entre 1 y 12");
        }
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
}
