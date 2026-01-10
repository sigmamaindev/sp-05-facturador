using Microsoft.EntityFrameworkCore;
using Core.Constants;
using Core.Entities;
using Core.Interfaces.Services.IUtilService;
using Core.Interfaces.Services.IAPService;
using Core.Interfaces.Repository;
using Core.DTOs;
using Core.DTOs.APDto;

namespace Infrastructure.Data;

public class AccountsPayableRepository(
    StoreContext context,
    IUserContextService currentUser,
    IAPDtoFactory apDtoFactory) : IAccountsPayableRepository
{
    public async Task<ApiResponse<APComplexResDto>> GetAccountsPayableByIdAsync(int id)
    {
        var response = new ApiResponse<APComplexResDto>();

        try
        {
            if (currentUser.BusinessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no asociado a esta usuario";
                response.Error = "Error de asociación";

                return response;
            }

            var existingAP = await context.AccountsPayables
                .Include(ap => ap.Supplier)
                .Include(ap => ap.Purchase).ThenInclude(p => p!.Business)
                .Include(ap => ap.Purchase).ThenInclude(p => p!.User)
                .Include(ap => ap.Transactions)
                .FirstOrDefaultAsync(ap => ap.Id == id);

            if (existingAP == null)
            {
                response.Success = false;
                response.Message = "Cuenta por pagar no encontrada";
                response.Error = "No existe una Cuenta por pagar con el ID especificado";

                return response;
            }

            response.Success = true;
            response.Message = "Cuenta por pagar obtenida correctamente";
            response.Data = apDtoFactory.APComplexRes(existingAP);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener la cuenta por pagar";
            response.Error = $"{ex.Message} | {ex.StackTrace}";
        }

        return response;
    }

    public async Task<ApiResponse<List<APSimpleResDto>>> GetAccountsPayablesAsync(string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<APSimpleResDto>>();

        try
        {
            if (currentUser.BusinessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no asociado a esta usuario";
                response.Error = "Error de asociación";

                return response;
            }

            var query = context.AccountsPayables
                .Include(ap => ap.Supplier)
                .Include(ap => ap.Purchase)
                .Where(ap =>
                    ap.BusinessId == currentUser.BusinessId &&
                    ap.Purchase != null &&
                    ap.Purchase.UserId == currentUser.UserId);

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();

                query = query.Where(ap =>
                    (ap.Purchase != null && EF.Functions.ILike(ap.Purchase.Sequential, $"%{keyword}%")) ||
                    (ap.Supplier != null && (
                        EF.Functions.ILike(ap.Supplier.BusinessName, $"%{keyword}%") ||
                        EF.Functions.ILike(ap.Supplier.Document, $"%{keyword}%")
                    )) ||
                    EF.Functions.ILike(ap.DocumentNumber, $"%{keyword}%") ||
                    EF.Functions.ILike(ap.Status, $"%{keyword}%")
                );
            }

            var total = await query.CountAsync();
            var skip = (page - 1) * limit;

            var apList = await query
                .OrderByDescending(ap => ap.IssueDate)
                .Skip(skip)
                .Take(limit)
                .ToListAsync();

            response.Success = true;
            response.Message = "Listado de cuentas por pagar obtenido correctamente.";
            response.Data = apList.Select(apDtoFactory.APSimpleRes).ToList();
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
            response.Message = "Error al obtener las cuentas por pagar";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<APComplexResDto>> AddTransactionAsync(int accountsPayableId, APTransactionCreateReqDto apTransactionCreateReqDto)
    {
        var response = new ApiResponse<APComplexResDto>();

        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            ValidateCurrentUser();

            if (!TryNormalizeAPTransactionType(apTransactionCreateReqDto.APTransactionType, out var normalizedType))
            {
                response.Success = false;
                response.Message = "Tipo de transacción inválido";
                response.Error = "Debe especificar un tipo de transacción válido";
                return response;
            }

            var amount = apTransactionCreateReqDto.Amount;

            if (normalizedType == APTransactionType.ADJUSTMENT)
            {
                if (amount == 0)
                {
                    response.Success = false;
                    response.Message = "Monto inválido";
                    response.Error = "El monto del ajuste no puede ser 0";
                    return response;
                }
            }
            else
            {
                if (amount <= 0)
                {
                    response.Success = false;
                    response.Message = "Monto inválido";
                    response.Error = "El monto debe ser mayor a 0";
                    return response;
                }
            }

            if (normalizedType == APTransactionType.PAYMENT && string.IsNullOrWhiteSpace(apTransactionCreateReqDto.PaymentMethod))
            {
                response.Success = false;
                response.Message = "Método de pago requerido";
                response.Error = "Debe especificar un método de pago para registrar un pago";
                return response;
            }

            var existingAP = await context.AccountsPayables
                .Include(ap => ap.Supplier)
                .Include(ap => ap.Purchase)
                .Include(ap => ap.Transactions)
                .FirstOrDefaultAsync(ap =>
                    ap.Id == accountsPayableId &&
                    ap.BusinessId == currentUser.BusinessId &&
                    ap.Purchase != null &&
                    ap.Purchase.UserId == currentUser.UserId);

            if (existingAP == null)
            {
                response.Success = false;
                response.Message = "Cuenta por pagar no encontrada";
                response.Error = "No existe una Cuenta por pagar con el ID especificado";
                return response;
            }

            var newTransaction = new APTransaction
            {
                APTransactionType = normalizedType,
                Amount = amount,
                PaymentMethod = string.IsNullOrWhiteSpace(apTransactionCreateReqDto.PaymentMethod)
                    ? null
                    : apTransactionCreateReqDto.PaymentMethod.Trim(),
                Reference = apTransactionCreateReqDto.Reference,
                Notes = apTransactionCreateReqDto.Notes,
                PaymentDetails = apTransactionCreateReqDto.PaymentDetails,
                AccountsPayable = existingAP,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.APTransactions.Add(newTransaction);

            RecalculateTotalsOrThrow(existingAP);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            response.Success = true;
            response.Message = "Transacción agregada correctamente";
            response.Data = apDtoFactory.APComplexRes(existingAP);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            response.Success = false;
            response.Message = "Error al agregar la transacción";
            response.Error = ex.Message;
        }

        return response;
    }

    private void ValidateCurrentUser()
    {
        if (currentUser.BusinessId == 0 || currentUser.UserId == 0)
        {
            throw new InvalidOperationException("Datos de autenticación incompletos");
        }
    }

    private static void RecalculateTotalsOrThrow(AccountsPayable accountsPayable)
    {
        var principal = accountsPayable.Transactions
            .Where(t => t.APTransactionType == APTransactionType.CHARGE)
            .Sum(t => t.Amount);

        principal += accountsPayable.Transactions
            .Where(t => t.APTransactionType == APTransactionType.ADJUSTMENT)
            .Sum(t => t.Amount);

        principal -= accountsPayable.Transactions
            .Where(t => t.APTransactionType == APTransactionType.CREDIT_NOTE)
            .Sum(t => t.Amount);

        if (principal < 0)
        {
            throw new InvalidOperationException("El total de la cuenta no puede ser negativo");
        }

        var totalPayments = accountsPayable.Transactions
            .Where(t => t.APTransactionType == APTransactionType.PAYMENT)
            .Sum(t => t.Amount);

        var balance = principal - totalPayments;

        if (balance < 0)
        {
            throw new InvalidOperationException("El pago excede el saldo actual");
        }

        accountsPayable.Total = principal;
        accountsPayable.Balance = balance;

        accountsPayable.Status = balance == 0
            ? AccountStatus.PAID
            : (totalPayments > 0 ? AccountStatus.PARTIALLY_PAID : AccountStatus.OPEN);
    }

    private static bool TryNormalizeAPTransactionType(string? transactionType, out string normalizedTransactionType)
    {
        normalizedTransactionType = string.Empty;

        if (string.IsNullOrWhiteSpace(transactionType))
        {
            return false;
        }

        var trimmed = transactionType.Trim();

        if (trimmed.Equals(APTransactionType.CHARGE, StringComparison.OrdinalIgnoreCase))
        {
            normalizedTransactionType = APTransactionType.CHARGE;
            return true;
        }

        if (trimmed.Equals(APTransactionType.PAYMENT, StringComparison.OrdinalIgnoreCase))
        {
            normalizedTransactionType = APTransactionType.PAYMENT;
            return true;
        }

        if (trimmed.Equals(APTransactionType.CREDIT_NOTE, StringComparison.OrdinalIgnoreCase))
        {
            normalizedTransactionType = APTransactionType.CREDIT_NOTE;
            return true;
        }

        if (trimmed.Equals(APTransactionType.ADJUSTMENT, StringComparison.OrdinalIgnoreCase))
        {
            normalizedTransactionType = APTransactionType.ADJUSTMENT;
            return true;
        }

        return false;
    }
}
