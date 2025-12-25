using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Core.DTOs;
using Core.DTOs.PurchaseDto;
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
            var establishment = await validate.ValidateEstablishmentAsync(currentUser.EstablishmentId);
            var emissionPoint = await validate.ValidateEmissionPointAsync(currentUser.EmissionPointId);
            var user = await validate.ValidateUserAsync(currentUser.UserId);

            var ecTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow,
              TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil"));

            var newPurchase = edition.BuildPurchase(
                purchaseCreateReqDto,
                supplier,
                business,
                establishment,
                emissionPoint,
                user,
                ecTime);

            await edition.AddPurchaseDetailsAsync(newPurchase, purchaseCreateReqDto.Details);

            var totals = calc.Calculate(newPurchase);

            newPurchase.SubtotalWithoutTaxes = totals.SubtotalWithoutTaxes;
            newPurchase.SubtotalWithTaxes = totals.SubtotalWithTaxes;
            newPurchase.DiscountTotal = totals.DiscountTotal;
            newPurchase.TaxTotal = totals.TaxTotal;
            newPurchase.TotalPurchase = totals.SubtotalWithTaxes;

            context.Purchases.Add(newPurchase);

            await context.SaveChangesAsync();

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

    public async Task<ApiResponse<PurchaseSimpleResDto>> GetPurchaseByIdAsync(int id)
    {
        var response = new ApiResponse<PurchaseSimpleResDto>();

        try
        {
            ValidateCurrentUser();

            var purchase = await context.Purchases
                .Include(p => p.PurchaseDetails)
                .FirstOrDefaultAsync(p =>
                    p.Id == id &&
                    p.BusinessId == currentUser.BusinessId &&
                    p.EstablishmentId == currentUser.EstablishmentId);

            if (purchase == null)
            {
                response.Success = false;
                response.Message = "Compra no encontrada";
                response.Error = "No existe una compra con el ID especificado";

                return response;
            }

            response.Success = true;
            response.Message = "Compra obtenida correctamente";
            response.Data = MapPurchaseRes(purchase);
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
                .Include(p => p.PurchaseDetails)
                .Where(p =>
                    p.BusinessId == currentUser.BusinessId &&
                    p.EstablishmentId == currentUser.EstablishmentId);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(p =>
                    EF.Functions.ILike(p.DocumentNumber, $"%{keyword}%") ||
                    EF.Functions.ILike(p.Reference, $"%{keyword}%"));
            }

            var total = await query.CountAsync();
            var purchases = await query
                .OrderByDescending(p => p.PurchaseDate)
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
            Sequential = purchase.Sequential,
            AccessKey = purchase.AccessKey,
            Environment = purchase.Environment,
            ReceiptType = purchase.ReceiptType,
            Status = purchase.Status,
            IsElectronic = purchase.IsElectronic,
            BusinessId = purchase.BusinessId,
            EstablishmentId = purchase.EstablishmentId,
            EmissionPointId = purchase.EmissionPointId,
            SupplierId = purchase.SupplierId,
            PurchaseDate = purchase.PurchaseDate,
            DocumentNumber = purchase.DocumentNumber,
            Reference = purchase.Reference,
            SubtotalWithoutTaxes = purchase.SubtotalWithoutTaxes,
            SubtotalWithTaxes = purchase.SubtotalWithTaxes,
            DiscountTotal = purchase.DiscountTotal,
            TaxTotal = purchase.TaxTotal,
            TotalPurchase = purchase.TotalPurchase,
            Details = [.. purchase.PurchaseDetails.Select(detail => new PurchaseDetailResDto
            {
                ProductId = detail.ProductId,
                WarehouseId = detail.WarehouseId,
                TaxId = detail.TaxId,
                Quantity = detail.Quantity,
                UnitCost = detail.UnitCost,
                Discount = detail.Discount,
                Subtotal = detail.Subtotal,
                TaxRate = detail.TaxRate,
                TaxValue = detail.TaxValue,
                Total = detail.Total
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
