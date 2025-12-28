using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Core.Entities;
using Core.Constants;
using Core.DTOs.ARDto;
using Core.Interfaces.Services.IARService;

namespace Infrastructure.Services.ARService;

public class AccountsReceivableService(StoreContext context) : IAccountsReceivableService
{
    public async Task<AccountsReceivable> UpsertFromInvoiceAsync(Invoice invoice, ARCreateFromInvoiceReqDto aRCreateFromInvoiceReqDto)
    {
        var ecTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil"));

        var issueDate = invoice.InvoiceDate == default ? ecTime : invoice.InvoiceDate;
        var dueDate = issueDate.AddDays(aRCreateFromInvoiceReqDto.TermDays);

        if (aRCreateFromInvoiceReqDto.TermDays < 0)
        {
            throw new Exception("TermDays no puede ser negativo");
        }

        if (string.IsNullOrWhiteSpace(aRCreateFromInvoiceReqDto.PaymentMethod))
        {
            throw new Exception("PaymentMethod requerido");
        }

        var initialPay = aRCreateFromInvoiceReqDto.InitialPaymentAmount.GetValueOrDefault(0);

        if (initialPay < 0)
        {
            throw new Exception("El pago inicial no puede ser negativo");
        }

        if (initialPay > 0 && string.IsNullOrWhiteSpace(aRCreateFromInvoiceReqDto.InitialPaymentMethodCode))
        {
            throw new Exception("El método de pago es requerido si hay abono inicial");
        }

        var accountReceivable = await context.AccountsReceivables
        .Include(x => x.Transactions)
        .FirstOrDefaultAsync(
            x =>
            x.InvoiceId == invoice.Id &&
            x.BusinessId == invoice.BusinessId);

        if (accountReceivable == null)
        {
            accountReceivable = new AccountsReceivable
            {
                InvoiceId = invoice.Id,
                BusinessId = invoice.BusinessId,
                CustomerId = invoice.CustomerId,
                IssueDate = issueDate,
                DueDate = dueDate,
                ExpectedPaymentDate = null,
                OriginalAmount = invoice.TotalInvoice,
                Status = AccountStatus.OPEN,
            };

            await context.AddAsync(accountReceivable);

            await context.SaveChangesAsync();

            var charge = new ARTransaction
            {
                ARTransactionType = ARTransactionType.CHARGE,
                Amount = invoice.TotalInvoice,
                PaymentMethod = null,
                Reference = aRCreateFromInvoiceReqDto.Reference,
                Notes = aRCreateFromInvoiceReqDto.Notes,
                AccountReceivableId = accountReceivable.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.AddAsync(charge);

            accountReceivable.Transactions.Add(charge);
        }
        else
        {
            accountReceivable.DueDate = dueDate;
            accountReceivable.ExpectedPaymentDate = aRCreateFromInvoiceReqDto.ExpectedPaymentDate;
        }

        if (initialPay > 0)
        {
            var payment = new ARTransaction
            {
                ARTransactionType = ARTransactionType.PAYMENT,
                Amount = initialPay,
                PaymentMethod = aRCreateFromInvoiceReqDto.InitialPaymentMethodCode,
                Reference = aRCreateFromInvoiceReqDto.Reference,
                Notes = aRCreateFromInvoiceReqDto.Notes,
                AccountReceivableId = accountReceivable.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await context.AddAsync(payment);

            accountReceivable.Transactions.Add(payment);
        }

        var totalCharge = accountReceivable.Transactions
        .Where(t => t.ARTransactionType == ARTransactionType.CHARGE)
        .Sum(t => t.Amount);

        var totalPayment = accountReceivable.Transactions
            .Where(t => t.ARTransactionType == ARTransactionType.PAYMENT)
            .Sum(t => t.Amount);

        var balance = totalCharge - totalPayment;

        if (balance < 0) balance = 0;

        accountReceivable.OriginalAmount = totalCharge;
        accountReceivable.Balance = balance;

        accountReceivable.Status = balance == 0
            ? AccountStatus.PAID
            : (totalPayment > 0 ? AccountStatus.PARTIALLY_PAID : AccountStatus.OPEN);

        await context.SaveChangesAsync();

        return accountReceivable;
    }
}
