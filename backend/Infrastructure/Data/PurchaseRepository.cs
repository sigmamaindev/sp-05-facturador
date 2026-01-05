using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Core.DTOs;
using Core.DTOs.PurchaseDto;
using Core.DTOs.SupplierDto;
using Core.Interfaces.Repository;
using Core.Interfaces.Services.IUtilService;
using Core.Interfaces.Services.IKardexService;
using Core.Interfaces.Services.IPurchaseService;

namespace Infrastructure.Data;

public class PurchaseRepository(
    StoreContext context,
    IUserContextService currentUser,
    IPurchaseValidationService validate,
    IPurchaseEditionService edition,
    IPurchaseCalculationService calc,
    IKardexService kardex) : IPurchaseRepository
{
    public async Task<ApiResponse<PurchaseSimpleResDto>> CreatePurchaseAsync(PurchaseCreateReqDto purchaseCreateReqDto)
    {
        var response = new ApiResponse<PurchaseSimpleResDto>();

        using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            ValidateCurrentUser();

            var supplier = await validate.ValidateSupplierAsync(purchaseCreateReqDto.SupplierId);
            var business = await validate.ValidateBusinessAsync(currentUser.BusinessId);
            var user = await validate.ValidateUserAsync(currentUser.UserId);

            var ecTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow,
              TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil"));

            var newPurchase = edition.BuildPurchase(
                purchaseCreateReqDto,
                user,
                business,
                supplier,
                ecTime);

            await edition.AddPurchaseDetailsAsync(newPurchase, purchaseCreateReqDto.Details);

            var totals = calc.Calculate(newPurchase);

            newPurchase.SubtotalWithoutTaxes = totals.SubtotalWithoutTaxes;
            newPurchase.SubtotalWithTaxes = totals.SubtotalWithTaxes;
            newPurchase.DiscountTotal = totals.DiscountTotal;
            newPurchase.TaxTotal = totals.TaxTotal;
            newPurchase.TotalPurchase = totals.SubtotalWithTaxes;

            context.Purchases.Add(newPurchase);

            await kardex.IncreaseStockForPurchaseAsync(newPurchase);

            await context.SaveChangesAsync();

            await transaction.CommitAsync();

            response.Success = true;
            response.Message = "Compra creada correctamente";
            response.Data = MapPurchaseRes(newPurchase);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            response.Success = false;
            response.Message = "Error al crear la compra";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<PurchaseComplexResDto>> GetPurchaseByIdAsync(int id)
    {
        var response = new ApiResponse<PurchaseComplexResDto>();

        try
        {
            ValidateCurrentUser();

            var purchase = await context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseDetails)
                    .ThenInclude(d => d.Product)
                .Include(p => p.PurchaseDetails)
                    .ThenInclude(d => d.UnitMeasure)
                .Include(p => p.PurchaseDetails)
                    .ThenInclude(d => d.Warehouse)
                .Include(p => p.PurchaseDetails)
                    .ThenInclude(d => d.Tax)
                .FirstOrDefaultAsync(p =>
                    p.Id == id &&
                    p.BusinessId == currentUser.BusinessId);

            if (purchase == null)
            {
                response.Success = false;
                response.Message = "Compra no encontrada";
                response.Error = "No existe una compra con el ID especificado";

                return response;
            }

            response.Success = true;
            response.Message = "Compra obtenida correctamente";
            response.Data = MapPurchaseComplexRes(purchase);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener la compra";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<List<PurchaseSimpleResDto>>> GetPurchasesAsync(string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<PurchaseSimpleResDto>>();

        try
        {
            ValidateCurrentUser();

            var query = context.Purchases
                .Include(p => p.Supplier)
                .Include(p => p.PurchaseDetails)
                .Where(p =>
                    p.BusinessId == currentUser.BusinessId);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(p =>
                    EF.Functions.ILike(p.BusinessName, $"%{keyword}%"));
            }

            var total = await query.CountAsync();
            var purchases = await query
                .OrderByDescending(p => p.IssueDate)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            response.Success = true;
            response.Message = "Compras obtenidas correctamente";
            response.Data = [.. purchases.Select(MapPurchaseRes)];
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
            response.Message = "Error al obtener las compras";
            response.Error = ex.Message;
        }

        return response;
    }

    private PurchaseSimpleResDto MapPurchaseRes(Purchase purchase)
    {
        return new PurchaseSimpleResDto
        {
            Id = purchase.Id,
            Environment = purchase.Environment,
            EmissionTypeCode = purchase.EmissionTypeCode,
            BusinessName = purchase.BusinessName,
            Name = purchase.Name,
            Document = purchase.Document,
            AccessKey = purchase.AccessKey,
            ReceiptType = purchase.ReceiptType,
            EstablishmentCode = purchase.EstablishmentCode,
            EmissionPointCode = purchase.EmissionPointCode,
            Sequential = purchase.Sequential,
            MainAddress = purchase.MainAddress,
            IssueDate = purchase.IssueDate,
            EstablishmentAddress = purchase.EstablishmentAddress,
            SpecialTaxpayer = purchase.SpecialTaxpayer,
            MandatoryAccounting = purchase.MandatoryAccounting,
            TypeDocumentSubjectDetained = purchase.TypeDocumentSubjectDetained,
            TypeSubjectDetained = purchase.TypeSubjectDetained ?? string.Empty,
            RelatedParty = purchase.RelatedParty,
            BusinessNameSubjectDetained = purchase.BusinessNameSubjectDetained ?? string.Empty,
            DocumentSubjectDetained = purchase.DocumentSubjectDetained ?? string.Empty,
            FiscalPeriod = purchase.FiscalPeriod ?? string.Empty,
            SupplierId = purchase.SupplierId,
            Supplier = purchase.Supplier == null
                ? null
                : new SupplierResDto
                {
                    Id = purchase.Supplier.Id,
                    BusinessName = purchase.Supplier.BusinessName,
                    Document = purchase.Supplier.Document,
                    Address = purchase.Supplier.Address,
                    Email = purchase.Supplier.Email,
                    Cellphone = purchase.Supplier.Cellphone,
                    Telephone = purchase.Supplier.Telephone,
                    IsActive = purchase.Supplier.IsActive,
                    CreatedAt = purchase.Supplier.CreatedAt
                },
            Status = purchase.Status,
            IsElectronic = purchase.IsElectronic,
            AuthorizationNumber = purchase.AuthorizationNumber,
            AuthorizationDate = purchase.AuthorizationDate,
            SubtotalWithoutTaxes = purchase.SubtotalWithoutTaxes,
            SubtotalWithTaxes = purchase.SubtotalWithTaxes,
            DiscountTotal = purchase.DiscountTotal,
            TaxTotal = purchase.TaxTotal,
            TotalPurchase = purchase.TotalPurchase,
        };
    }

    private PurchaseComplexResDto MapPurchaseComplexRes(Purchase purchase)
    {
        return new PurchaseComplexResDto
        {
            Id = purchase.Id,
            Environment = purchase.Environment,
            EmissionTypeCode = purchase.EmissionTypeCode,
            BusinessName = purchase.BusinessName,
            Name = purchase.Name,
            Document = purchase.Document,
            AccessKey = purchase.AccessKey,
            ReceiptType = purchase.ReceiptType,
            EstablishmentCode = purchase.EstablishmentCode,
            EmissionPointCode = purchase.EmissionPointCode,
            Sequential = purchase.Sequential,
            MainAddress = purchase.MainAddress,
            IssueDate = purchase.IssueDate,
            EstablishmentAddress = purchase.EstablishmentAddress,
            SpecialTaxpayer = purchase.SpecialTaxpayer,
            MandatoryAccounting = purchase.MandatoryAccounting,
            TypeDocumentSubjectDetained = purchase.TypeDocumentSubjectDetained,
            TypeSubjectDetained = purchase.TypeSubjectDetained ?? string.Empty,
            RelatedParty = purchase.RelatedParty,
            BusinessNameSubjectDetained = purchase.BusinessNameSubjectDetained ?? string.Empty,
            DocumentSubjectDetained = purchase.DocumentSubjectDetained ?? string.Empty,
            FiscalPeriod = purchase.FiscalPeriod ?? string.Empty,
            SupplierId = purchase.SupplierId,
            Supplier = purchase.Supplier == null
                ? null
                : new SupplierResDto
                {
                    Id = purchase.Supplier.Id,
                    BusinessName = purchase.Supplier.BusinessName,
                    Document = purchase.Supplier.Document,
                    Address = purchase.Supplier.Address,
                    Email = purchase.Supplier.Email,
                    Cellphone = purchase.Supplier.Cellphone,
                    Telephone = purchase.Supplier.Telephone,
                    IsActive = purchase.Supplier.IsActive,
                    CreatedAt = purchase.Supplier.CreatedAt
                },
            Status = purchase.Status,
            IsElectronic = purchase.IsElectronic,
            AuthorizationNumber = purchase.AuthorizationNumber,
            AuthorizationDate = purchase.AuthorizationDate,
            SubtotalWithoutTaxes = purchase.SubtotalWithoutTaxes,
            SubtotalWithTaxes = purchase.SubtotalWithTaxes,
            DiscountTotal = purchase.DiscountTotal,
            TaxTotal = purchase.TaxTotal,
            TotalPurchase = purchase.TotalPurchase,
            Details = [.. purchase.PurchaseDetails
                .OrderBy(d => d.Id)
                .Select(d => new PurchaseDetailResDto
                {
                    Id = d.Id,
                    PurchaseId = d.PurchaseId,
                    ProductId = d.ProductId,
                    ProductCode = d.Product?.Sku ?? string.Empty,
                    ProductName = d.Product?.Name ?? string.Empty,
                    WarehouseId = d.WarehouseId,
                    WarehouseCode = d.Warehouse?.Code ?? string.Empty,
                    WarehouseName = d.Warehouse?.Name ?? string.Empty,
                    UnitMeasureId = d.UnitMeasureId,
                    UnitMeasureCode = d.UnitMeasure?.Code ?? string.Empty,
                    UnitMeasureName = d.UnitMeasure?.Name ?? string.Empty,
                    TaxId = d.TaxId,
                    TaxCode = d.Tax?.Code ?? string.Empty,
                    TaxName = d.Tax?.Name ?? string.Empty,
                    TaxRate = d.TaxRate,
                    TaxValue = d.TaxValue,
                    Quantity = d.Quantity,
                    NetWeight = d.NetWeight,
                    GrossWeight = d.GrossWeight,
                    UnitCost = d.UnitCost,
                    Discount = d.Discount,
                    Subtotal = d.Subtotal,
                    Total = d.Total
                })]
        };
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
