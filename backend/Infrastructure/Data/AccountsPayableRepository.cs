using Microsoft.EntityFrameworkCore;
using Core.Constants;
using Core.Entities;
using Core.Interfaces.Services.IUtilService;
using Core.Interfaces.Services.IAPService;
using Core.Interfaces.Repository;
using Core.DTOs;
using Core.DTOs.APDto;
using Core.DTOs.SupplierDto;

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
                    ap.Purchase != null);

            if (!currentUser.IsAdmin)
            {
                query = query.Where(ap => ap.Purchase!.UserId == currentUser.UserId);
            }

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

    public async Task<ApiResponse<List<APSupplierSummaryResDto>>> GetAccountsPayablesBySupplierAsync(string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<APSupplierSummaryResDto>>();

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
                .AsNoTracking()
                .Where(ap =>
                    ap.BusinessId == currentUser.BusinessId &&
                    ap.Supplier != null &&
                    ap.Purchase != null);

            if (!currentUser.IsAdmin)
            {
                query = query.Where(ap => ap.Purchase!.UserId == currentUser.UserId);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalizedKeyword = keyword.Trim().ToLowerInvariant();

                query = query.Where(ap =>
                    EF.Functions.ILike(ap.Supplier!.BusinessName, $"%{normalizedKeyword}%") ||
                    EF.Functions.ILike(ap.Supplier!.Document, $"%{normalizedKeyword}%"));
            }

            var supplierGroups = query.GroupBy(ap => new
            {
                ap.Supplier!.Id,
                ap.Supplier.BusinessName,
                ap.Supplier.Document,
                ap.Supplier.Address,
                ap.Supplier.Email,
                ap.Supplier.Cellphone,
                ap.Supplier.Telephone,
                ap.Supplier.IsActive,
                ap.Supplier.CreatedAt
            });

            var total = await supplierGroups.CountAsync();
            var skip = (page - 1) * limit;

            var suppliers = await supplierGroups
                .OrderBy(g => g.Key.BusinessName)
                .Skip(skip)
                .Take(limit)
                .Select(g => new APSupplierSummaryResDto
                {
                    Supplier = new SupplierResDto
                    {
                        Id = g.Key.Id,
                        BusinessName = g.Key.BusinessName,
                        Document = g.Key.Document,
                        Address = g.Key.Address,
                        Email = g.Key.Email,
                        Cellphone = g.Key.Cellphone,
                        Telephone = g.Key.Telephone,
                        IsActive = g.Key.IsActive,
                        CreatedAt = g.Key.CreatedAt
                    },
                    TotalBalance = g.Sum(ap => ap.Balance)
                })
                .ToListAsync();

            response.Success = true;
            response.Message = "Listado de cuentas por pagar por proveedor obtenido correctamente.";
            response.Data = suppliers;
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
            response.Message = "Error al obtener las cuentas por pagar por proveedor";
            response.Error = ex.Message;

            return response;
        }
    }

    public async Task<ApiResponse<List<APSimpleResDto>>> GetAccountsPayablesBySupplierIdAsync(int supplierId, string? keyword, int page, int limit)
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
                .AsNoTracking()
                .Include(ap => ap.Supplier)
                .Include(ap => ap.Purchase)
                .Where(ap =>
                    ap.BusinessId == currentUser.BusinessId &&
                    ap.SupplierId == supplierId &&
                    ap.Purchase != null);

            if (!currentUser.IsAdmin)
            {
                query = query.Where(ap => ap.Purchase!.UserId == currentUser.UserId);
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalizedKeyword = keyword.Trim().ToLowerInvariant();

                query = query.Where(ap =>
                    (ap.Purchase != null && EF.Functions.ILike(ap.Purchase.Sequential, $"%{normalizedKeyword}%")) ||
                    EF.Functions.ILike(ap.DocumentNumber, $"%{normalizedKeyword}%") ||
                    EF.Functions.ILike(ap.Status, $"%{normalizedKeyword}%"));
            }

            var total = await query.CountAsync();
            var skip = (page - 1) * limit;

            var apList = await query
                .OrderByDescending(ap => ap.IssueDate)
                .Skip(skip)
                .Take(limit)
                .ToListAsync();

            response.Success = true;
            response.Message = "Listado de cuentas por pagar del proveedor obtenido correctamente.";
            response.Data = apList.Select(apDtoFactory.APSimpleRes).ToList();
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
            response.Message = "Error al obtener las cuentas por pagar del proveedor";
            response.Error = ex.Message;

            return response;
        }
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

            var apQuery = context.AccountsPayables
                .Include(ap => ap.Supplier)
                .Include(ap => ap.Purchase)
                .Include(ap => ap.Transactions)
                .Where(ap =>
                    ap.Id == accountsPayableId &&
                    ap.BusinessId == currentUser.BusinessId &&
                    ap.Purchase != null);

            if (!currentUser.IsAdmin)
            {
                apQuery = apQuery.Where(ap => ap.Purchase!.UserId == currentUser.UserId);
            }

            var existingAP = await apQuery.FirstOrDefaultAsync();

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

    public async Task<ApiResponse<List<APSimpleResDto>>> AddBulkPaymentsAsync(APBulkPaymentCreateReqDto request)
    {
        var response = new ApiResponse<List<APSimpleResDto>>();

        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            ValidateCurrentUser();

            if (request.Items.Count == 0)
            {
                response.Success = false;
                response.Message = "Listado de pagos vacío";
                response.Error = "Debe enviar al menos un pago";
                return response;
            }

            if (string.IsNullOrWhiteSpace(request.PaymentMethod))
            {
                response.Success = false;
                response.Message = "Método de pago requerido";
                response.Error = "Debe especificar un método de pago para registrar un pago";
                return response;
            }

            var paymentMethod = request.PaymentMethod.Trim();
            var notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();

            var paymentsByApId = request.Items
                .GroupBy(i => i.AccountsPayableId)
                .Select(g => new { AccountsPayableId = g.Key, Amount = g.Sum(x => x.Amount) })
                .ToDictionary(x => x.AccountsPayableId, x => x.Amount);

            if (paymentsByApId.Any(p => p.Key <= 0))
            {
                response.Success = false;
                response.Message = "ID inválido";
                response.Error = "Todos los IDs de cuentas por pagar deben ser mayores a 0";
                return response;
            }

            if (paymentsByApId.Any(p => p.Value <= 0))
            {
                response.Success = false;
                response.Message = "Monto inválido";
                response.Error = "Todos los montos deben ser mayores a 0";
                return response;
            }

            var apIds = paymentsByApId.Keys.ToList();

            var bulkQuery = context.AccountsPayables
                .Include(ap => ap.Supplier)
                .Include(ap => ap.Purchase)
                .Include(ap => ap.Transactions)
                .Where(ap =>
                    ap.BusinessId == currentUser.BusinessId &&
                    ap.Purchase != null &&
                    apIds.Contains(ap.Id));

            if (!currentUser.IsAdmin)
            {
                bulkQuery = bulkQuery.Where(ap => ap.Purchase!.UserId == currentUser.UserId);
            }

            var accountsPayables = await bulkQuery.ToListAsync();

            var foundIds = accountsPayables.Select(ap => ap.Id).ToHashSet();
            var missingIds = apIds.Where(id => !foundIds.Contains(id)).ToList();

            if (missingIds.Count != 0)
            {
                response.Success = false;
                response.Message = "Cuenta por pagar no encontrada";
                response.Error = $"No existe una Cuenta por pagar con los IDs: {string.Join(", ", missingIds)}";
                return response;
            }

            foreach (var accountsPayable in accountsPayables)
            {
                var amount = paymentsByApId[accountsPayable.Id];

                var newTransaction = new APTransaction
                {
                    APTransactionType = APTransactionType.PAYMENT,
                    Amount = amount,
                    PaymentMethod = paymentMethod,
                    Notes = notes,
                    AccountsPayableId = accountsPayable.Id,
                    AccountsPayable = accountsPayable,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                accountsPayable.Transactions.Add(newTransaction);
                context.APTransactions.Add(newTransaction);

                RecalculateTotalsOrThrow(accountsPayable);
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            response.Success = true;
            response.Message = "Pagos registrados correctamente";
            response.Data = accountsPayables.Select(apDtoFactory.APSimpleRes).ToList();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            response.Success = false;
            response.Message = "Error al registrar los pagos";
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
