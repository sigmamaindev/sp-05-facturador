using Microsoft.EntityFrameworkCore;
using Core.Constants;
using Core.Entities;
using Core.Interfaces.Services.IUtilService;
using Core.Interfaces.Services.IARService;
using Core.Interfaces.Repository;
using Core.DTOs;
using Core.DTOs.ARDto;
using Core.DTOs.CustomerDto;

namespace Infrastructure.Data;

public class AccountsReceivableRepository(
    StoreContext context,
    IUserContextService currentUser,
    IARDtoFactory aRDtoFactory) : IAccountsReceivableRepository
{
    public async Task<ApiResponse<ARComplexResDto>> GetAccountsReceivableByIdAsync(int id)
    {
        var response = new ApiResponse<ARComplexResDto>();

        try
        {
            if (currentUser.BusinessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no asociado a esta usuario";
                response.Error = "Error de asociación";

                return response;
            }

            var existingAR = await context.AccountsReceivables
            .Include(ar => ar.Customer)
            .Include(ar => ar.Invoice).ThenInclude(i => i!.Business)
            .Include(ar => ar.Invoice).ThenInclude(i => i!.Establishment)
            .Include(ar => ar.Invoice).ThenInclude(i => i!.EmissionPoint)
            .Include(ar => ar.Invoice).ThenInclude(i => i!.User)
            .Include(ar => ar.Transactions)
            .FirstOrDefaultAsync(
                ar =>
                ar.Id == id);

            if (existingAR == null)
            {
                response.Success = false;
                response.Message = "Cuenta por cobrar no encontrada";
                response.Error = "No existe una Cuenta por cobrar con el ID especificado";

                return response;
            }

            response.Success = true;
            response.Message = "Cuenta por cobrar obtenida correctamente";
            response.Data = aRDtoFactory.ARComplexRes(existingAR);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener la cuenta por cobrar";
            response.Error = $"{ex.Message} | {ex.StackTrace}";
        }

        return response;
    }

    public async Task<ApiResponse<List<ARSimpleResDto>>> GetAccountsReceivablesAsync(string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<ARSimpleResDto>>();

        try
        {
            if (currentUser.BusinessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no asociado a esta usuario";
                response.Error = "Error de asociación";

                return response;
            }

            var query = context.AccountsReceivables
            .Include(ar => ar.Customer)
            .Include(ar => ar.Invoice).ThenInclude(i => i!.Business)
            .Include(ar => ar.Invoice).ThenInclude(i => i!.Establishment)
            .Include(ar => ar.Invoice).ThenInclude(i => i!.EmissionPoint)
            .Include(ar => ar.Invoice).ThenInclude(i => i!.User)
            .Where(
                ar =>
                ar.BusinessId == currentUser.BusinessId &&
                ar.Invoice != null &&
                ar.Invoice.EstablishmentId == currentUser.EstablishmentId &&
                ar.Invoice.EmissionPointId == currentUser.EmissionPointId &&
                ar.Invoice.UserId == currentUser.UserId);

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();

                query = query.Where(ar =>
                    (ar.Invoice != null && EF.Functions.ILike(ar.Invoice.Sequential, $"%{keyword}%")) ||
                    (ar.Customer != null && (
                        EF.Functions.ILike(ar.Customer.Name, $"%{keyword}%") ||
                        EF.Functions.ILike(ar.Customer.Document, $"%{keyword}%")
                    )) ||
                    EF.Functions.ILike(ar.Status, $"%{keyword}%")
                );
            }

            var total = await query.CountAsync();
            var skip = (page - 1) * limit;

            var arList = await query
                .OrderByDescending(ar => ar.IssueDate)
                .Skip(skip)
                .Take(limit)
                .ToListAsync();

            var accountsReceivables = arList.Select(aRDtoFactory.ARSimpleRes).ToList();

            response.Success = true;
            response.Message = "Listado de cuentas por cobrar obtenido correctamente.";
            response.Data = accountsReceivables;
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
            response.Message = "Error al obtener las cuentas por cobrar";
            response.Error = ex.Message;

            return response;
        }
    }

    public async Task<ApiResponse<List<ARCustomerSummaryResDto>>> GetAccountsReceivablesByCustomerAsync(string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<ARCustomerSummaryResDto>>();

        try
        {
            if (currentUser.BusinessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no asociado a esta usuario";
                response.Error = "Error de asociación";

                return response;
            }

            var query = context.AccountsReceivables
                .AsNoTracking()
                .Where(ar =>
                    ar.BusinessId == currentUser.BusinessId &&
                    ar.Customer != null &&
                    ar.Invoice != null &&
                    ar.Invoice.EstablishmentId == currentUser.EstablishmentId &&
                    ar.Invoice.EmissionPointId == currentUser.EmissionPointId &&
                    ar.Invoice.UserId == currentUser.UserId);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalizedKeyword = keyword.Trim().ToLowerInvariant();

                query = query.Where(ar =>
                    EF.Functions.ILike(ar.Customer!.Name, $"%{normalizedKeyword}%") ||
                    EF.Functions.ILike(ar.Customer!.Document, $"%{normalizedKeyword}%"));
            }

            var customerGroups = query.GroupBy(ar => new
            {
                ar.Customer!.Id,
                ar.Customer.Document,
                ar.Customer.DocumentType,
                ar.Customer.Name,
                ar.Customer.Email,
                ar.Customer.Address,
                ar.Customer.Cellphone,
                ar.Customer.Telephone,
                ar.Customer.IsActive,
                ar.Customer.CreatedAt
            });

            var total = await customerGroups.CountAsync();
            var skip = (page - 1) * limit;

            var customers = await customerGroups
                .OrderBy(g => g.Key.Name)
                .Skip(skip)
                .Take(limit)
                .Select(g => new ARCustomerSummaryResDto
                {
                    Customer = new CustomerResDto
                    {
                        Id = g.Key.Id,
                        Document = g.Key.Document,
                        DocumentType = g.Key.DocumentType,
                        Name = g.Key.Name,
                        Email = g.Key.Email,
                        Address = g.Key.Address,
                        Cellphone = g.Key.Cellphone,
                        Telephone = g.Key.Telephone,
                        IsActive = g.Key.IsActive,
                        CreatedAt = g.Key.CreatedAt
                    },
                    TotalBalance = g.Sum(ar => ar.Balance)
                })
                .ToListAsync();

            response.Success = true;
            response.Message = "Listado de cuentas por cobrar por cliente obtenido correctamente.";
            response.Data = customers;
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
            response.Message = "Error al obtener las cuentas por cobrar por cliente";
            response.Error = ex.Message;

            return response;
        }
    }

    public async Task<ApiResponse<List<ARSimpleResDto>>> GetAccountsReceivablesByCustomerIdAsync(int customerId, string? keyword, int page, int limit)
    {
        var response = new ApiResponse<List<ARSimpleResDto>>();

        try
        {
            if (currentUser.BusinessId == 0)
            {
                response.Success = false;
                response.Message = "Negocio no asociado a esta usuario";
                response.Error = "Error de asociación";

                return response;
            }

            var query = context.AccountsReceivables
                .AsNoTracking()
                .Include(ar => ar.Customer)
                .Include(ar => ar.Invoice).ThenInclude(i => i!.Business)
                .Include(ar => ar.Invoice).ThenInclude(i => i!.Establishment)
                .Include(ar => ar.Invoice).ThenInclude(i => i!.EmissionPoint)
                .Include(ar => ar.Invoice).ThenInclude(i => i!.User)
                .Where(ar =>
                    ar.BusinessId == currentUser.BusinessId &&
                    ar.CustomerId == customerId &&
                    ar.Invoice != null &&
                    ar.Invoice.EstablishmentId == currentUser.EstablishmentId &&
                    ar.Invoice.EmissionPointId == currentUser.EmissionPointId &&
                    ar.Invoice.UserId == currentUser.UserId);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var normalizedKeyword = keyword.Trim().ToLowerInvariant();

                query = query.Where(ar =>
                    (ar.Invoice != null && EF.Functions.ILike(ar.Invoice.Sequential, $"%{normalizedKeyword}%")) ||
                    EF.Functions.ILike(ar.Status, $"%{normalizedKeyword}%"));
            }

            var total = await query.CountAsync();
            var skip = (page - 1) * limit;

            var arList = await query
                .OrderByDescending(ar => ar.IssueDate)
                .Skip(skip)
                .Take(limit)
                .ToListAsync();

            response.Success = true;
            response.Message = "Listado de cuentas por cobrar del cliente obtenido correctamente.";
            response.Data = arList.Select(aRDtoFactory.ARSimpleRes).ToList();
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
            response.Message = "Error al obtener las cuentas por cobrar del cliente";
            response.Error = ex.Message;

            return response;
        }
    }

    public async Task<ApiResponse<List<ARSimpleResDto>>> AddBulkPaymentsAsync(ARBulkPaymentCreateReqDto request)
    {
        var response = new ApiResponse<List<ARSimpleResDto>>();

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

            var paymentsByArId = request.Items
                .GroupBy(i => i.AccountsReceivableId)
                .Select(g => new { AccountsReceivableId = g.Key, Amount = g.Sum(x => x.Amount) })
                .ToDictionary(x => x.AccountsReceivableId, x => x.Amount);

            if (paymentsByArId.Any(p => p.Key <= 0))
            {
                response.Success = false;
                response.Message = "ID inválido";
                response.Error = "Todos los IDs de cuentas por cobrar deben ser mayores a 0";
                return response;
            }

            if (paymentsByArId.Any(p => p.Value <= 0))
            {
                response.Success = false;
                response.Message = "Monto inválido";
                response.Error = "Todos los montos deben ser mayores a 0";
                return response;
            }

            var arIds = paymentsByArId.Keys.ToList();

            var accountsReceivables = await context.AccountsReceivables
                .Include(ar => ar.Customer)
                .Include(ar => ar.Invoice).ThenInclude(i => i!.Business)
                .Include(ar => ar.Invoice).ThenInclude(i => i!.Establishment)
                .Include(ar => ar.Invoice).ThenInclude(i => i!.EmissionPoint)
                .Include(ar => ar.Invoice).ThenInclude(i => i!.User)
                .Include(ar => ar.Transactions)
                .Where(ar =>
                    ar.BusinessId == currentUser.BusinessId &&
                    ar.Invoice != null &&
                    ar.Invoice.EstablishmentId == currentUser.EstablishmentId &&
                    ar.Invoice.EmissionPointId == currentUser.EmissionPointId &&
                    ar.Invoice.UserId == currentUser.UserId &&
                    arIds.Contains(ar.Id))
                .ToListAsync();

            var foundIds = accountsReceivables.Select(ar => ar.Id).ToHashSet();
            var missingIds = arIds.Where(id => !foundIds.Contains(id)).ToList();

            if (missingIds.Count != 0)
            {
                response.Success = false;
                response.Message = "Cuenta por cobrar no encontrada";
                response.Error = $"No existe una Cuenta por cobrar con los IDs: {string.Join(", ", missingIds)}";
                return response;
            }

            foreach (var accountsReceivable in accountsReceivables)
            {
                var amount = paymentsByArId[accountsReceivable.Id];

                var newTransaction = new ARTransaction
                {
                    ARTransactionType = ARTransactionType.PAYMENT,
                    Amount = amount,
                    PaymentMethod = paymentMethod,
                    Notes = notes,
                    AccountReceivableId = accountsReceivable.Id,
                    AccountsReceivable = accountsReceivable,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                accountsReceivable.Transactions.Add(newTransaction);
                context.ARTransactions.Add(newTransaction);

                RecalculateTotalsOrThrow(accountsReceivable);
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            response.Success = true;
            response.Message = "Pagos registrados correctamente";
            response.Data = accountsReceivables.Select(aRDtoFactory.ARSimpleRes).ToList();
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

    public async Task<ApiResponse<ARComplexResDto>> AddTransactionAsync(int accountsReceivableId, ARTransactionCreateReqDto aRTransactionCreateReqDto)
    {
        var response = new ApiResponse<ARComplexResDto>();

        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            ValidateCurrentUser();

            if (!TryNormalizeARTransactionType(aRTransactionCreateReqDto.ARTransactionType, out var normalizedType))
            {
                response.Success = false;
                response.Message = "Tipo de transacción inválido";
                response.Error = "Debe especificar un tipo de transacción válido";
                return response;
            }

            var amount = aRTransactionCreateReqDto.Amount;

            if (normalizedType == ARTransactionType.ADJUSTMENT)
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

            if (normalizedType == ARTransactionType.PAYMENT && string.IsNullOrWhiteSpace(aRTransactionCreateReqDto.PaymentMethod))
            {
                response.Success = false;
                response.Message = "Método de pago requerido";
                response.Error = "Debe especificar un método de pago para registrar un pago";
                return response;
            }

            var existingAR = await context.AccountsReceivables
                .Include(ar => ar.Customer)
                .Include(ar => ar.Invoice).ThenInclude(i => i!.Business)
                .Include(ar => ar.Invoice).ThenInclude(i => i!.Establishment)
                .Include(ar => ar.Invoice).ThenInclude(i => i!.EmissionPoint)
                .Include(ar => ar.Invoice).ThenInclude(i => i!.User)
                .Include(ar => ar.Transactions)
                .FirstOrDefaultAsync(ar =>
                    ar.Id == accountsReceivableId &&
                    ar.BusinessId == currentUser.BusinessId &&
                    ar.Invoice != null &&
                    ar.Invoice.EstablishmentId == currentUser.EstablishmentId &&
                    ar.Invoice.EmissionPointId == currentUser.EmissionPointId &&
                    ar.Invoice.UserId == currentUser.UserId);

            if (existingAR == null)
            {
                response.Success = false;
                response.Message = "Cuenta por cobrar no encontrada";
                response.Error = "No existe una Cuenta por cobrar con el ID especificado";
                return response;
            }

            var newTransaction = new ARTransaction
            {
                ARTransactionType = normalizedType,
                Amount = amount,
                PaymentMethod = string.IsNullOrWhiteSpace(aRTransactionCreateReqDto.PaymentMethod)
                    ? null
                    : aRTransactionCreateReqDto.PaymentMethod.Trim(),
                Reference = aRTransactionCreateReqDto.Reference,
                Notes = aRTransactionCreateReqDto.Notes,
                PaymentDetails = aRTransactionCreateReqDto.PaymentDetails,
                AccountReceivableId = existingAR.Id,
                AccountsReceivable = existingAR,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.ARTransactions.Add(newTransaction);

            RecalculateTotalsOrThrow(existingAR);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            response.Success = true;
            response.Message = "Transacción agregada correctamente";
            response.Data = aRDtoFactory.ARComplexRes(existingAR);
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
        if (currentUser.BusinessId == 0 ||
            currentUser.EstablishmentId == 0 ||
            currentUser.EmissionPointId == 0 ||
            currentUser.UserId == 0)
        {
            throw new InvalidOperationException("Datos de autenticación incompletos");
        }
    }

    private static void RecalculateTotalsOrThrow(AccountsReceivable accountsReceivable)
    {
        var principal = accountsReceivable.Transactions
            .Where(t => t.ARTransactionType == ARTransactionType.CHARGE)
            .Sum(t => t.Amount);

        principal += accountsReceivable.Transactions
            .Where(t => t.ARTransactionType == ARTransactionType.ADJUSTMENT)
            .Sum(t => t.Amount);

        principal -= accountsReceivable.Transactions
            .Where(t => t.ARTransactionType == ARTransactionType.CREDIT_NOTE)
            .Sum(t => t.Amount);

        if (principal < 0)
        {
            throw new InvalidOperationException("El total de la cuenta no puede ser negativo");
        }

        var totalPayments = accountsReceivable.Transactions
            .Where(t => t.ARTransactionType == ARTransactionType.PAYMENT)
            .Sum(t => t.Amount);

        var balance = principal - totalPayments;

        if (balance < 0)
        {
            throw new InvalidOperationException("El pago excede el saldo actual");
        }

        accountsReceivable.OriginalAmount = principal;
        accountsReceivable.Balance = balance;

        accountsReceivable.Status = balance == 0
            ? AccountStatus.PAID
            : (totalPayments > 0 ? AccountStatus.PARTIALLY_PAID : AccountStatus.OPEN);
    }

    private static bool TryNormalizeARTransactionType(string? transactionType, out string normalizedTransactionType)
    {
        normalizedTransactionType = string.Empty;

        if (string.IsNullOrWhiteSpace(transactionType))
        {
            return false;
        }

        var trimmed = transactionType.Trim();

        if (trimmed.Equals(ARTransactionType.CHARGE, StringComparison.OrdinalIgnoreCase))
        {
            normalizedTransactionType = ARTransactionType.CHARGE;
            return true;
        }

        if (trimmed.Equals(ARTransactionType.PAYMENT, StringComparison.OrdinalIgnoreCase))
        {
            normalizedTransactionType = ARTransactionType.PAYMENT;
            return true;
        }

        if (trimmed.Equals(ARTransactionType.CREDIT_NOTE, StringComparison.OrdinalIgnoreCase))
        {
            normalizedTransactionType = ARTransactionType.CREDIT_NOTE;
            return true;
        }

        if (trimmed.Equals(ARTransactionType.ADJUSTMENT, StringComparison.OrdinalIgnoreCase))
        {
            normalizedTransactionType = ARTransactionType.ADJUSTMENT;
            return true;
        }

        return false;
    }
}
