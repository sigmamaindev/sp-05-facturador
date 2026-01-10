using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Core.Constants;
using Core.DTOs.APDto;
using Core.Entities;
using Core.Interfaces.Services.IAPService;

namespace Infrastructure.Services.APService;

public class AccountsPayableService(StoreContext context) : IAccountsPayableService
{
    public async Task<AccountsPayable> UpsertFromPurchaseAsync(Purchase purchase, APCreateFromPurchaseReqDto apCreateFromPurchaseReqDto)
    {
        var ecTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("America/Guayaquil"));

        var issueDate = purchase.IssueDate == default ? ecTime : purchase.IssueDate;
        var dueDate = issueDate.AddDays(apCreateFromPurchaseReqDto.TermDays);

        if (apCreateFromPurchaseReqDto.TermDays < 0)
        {
            throw new Exception("TermDays no puede ser negativo");
        }

        var initialPay = apCreateFromPurchaseReqDto.InitialPaymentAmount.GetValueOrDefault(0);

        if (initialPay < 0)
        {
            throw new Exception("El pago inicial no puede ser negativo");
        }

        if (initialPay > 0 && string.IsNullOrWhiteSpace(apCreateFromPurchaseReqDto.InitialPaymentMethodCode))
        {
            throw new Exception("El método de pago es requerido si hay abono inicial");
        }

        AccountsPayable? accountsPayable = null;

        if (purchase.Id != 0)
        {
            accountsPayable = await context.AccountsPayables
                .Include(x => x.Transactions)
                .FirstOrDefaultAsync(x =>
                    x.PurchaseId == purchase.Id &&
                    x.BusinessId == purchase.BusinessId);
        }

        if (accountsPayable == null)
        {
            accountsPayable = new AccountsPayable
            {
                PurchaseId = purchase.Id,
                Purchase = purchase,
                BusinessId = purchase.BusinessId,
                SupplierId = purchase.SupplierId,
                UserId = purchase.UserId,
                DocumentNumber = purchase.Sequential,
                Subtotal = purchase.SubtotalWithoutTaxes,
                DiscountTotal = purchase.DiscountTotal,
                TaxTotal = purchase.TaxTotal,
                Total = purchase.TotalPurchase,
                Status = AccountStatus.OPEN,
                Notes = apCreateFromPurchaseReqDto.Notes,
                IssueDate = issueDate,
                DueDate = dueDate,
                ExpectedPaymentDate = apCreateFromPurchaseReqDto.ExpectedPaymentDate,
            };

            context.AccountsPayables.Add(accountsPayable);

            var charge = new APTransaction
            {
                APTransactionType = APTransactionType.CHARGE,
                Amount = purchase.TotalPurchase,
                PaymentMethod = null,
                Reference = apCreateFromPurchaseReqDto.Reference,
                Notes = apCreateFromPurchaseReqDto.Notes,
                AccountsPayable = accountsPayable,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.APTransactions.Add(charge);
        }
        else
        {
            accountsPayable.DocumentNumber = purchase.Sequential;
            accountsPayable.Subtotal = purchase.SubtotalWithoutTaxes;
            accountsPayable.DiscountTotal = purchase.DiscountTotal;
            accountsPayable.TaxTotal = purchase.TaxTotal;
            accountsPayable.Total = purchase.TotalPurchase;
            accountsPayable.DueDate = dueDate;
            accountsPayable.ExpectedPaymentDate = apCreateFromPurchaseReqDto.ExpectedPaymentDate;
            accountsPayable.Notes = apCreateFromPurchaseReqDto.Notes;
        }

        if (initialPay > 0)
        {
            var payment = new APTransaction
            {
                APTransactionType = APTransactionType.PAYMENT,
                Amount = initialPay,
                PaymentMethod = apCreateFromPurchaseReqDto.InitialPaymentMethodCode,
                Reference = apCreateFromPurchaseReqDto.Reference,
                Notes = apCreateFromPurchaseReqDto.Notes,
                AccountsPayable = accountsPayable,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            context.APTransactions.Add(payment);
            accountsPayable.Transactions.Add(payment);
        }

        var totalCharge = accountsPayable.Transactions
            .Where(t => t.APTransactionType == APTransactionType.CHARGE)
            .Sum(t => t.Amount);

        var totalPayment = accountsPayable.Transactions
            .Where(t => t.APTransactionType == APTransactionType.PAYMENT)
            .Sum(t => t.Amount);

        var balance = totalCharge - totalPayment;
        if (balance < 0) balance = 0;

        accountsPayable.Total = totalCharge;
        accountsPayable.Balance = balance;

        accountsPayable.Status = balance == 0
            ? AccountStatus.PAID
            : (totalPayment > 0 ? AccountStatus.PARTIALLY_PAID : AccountStatus.OPEN);

        return accountsPayable;
    }
}
