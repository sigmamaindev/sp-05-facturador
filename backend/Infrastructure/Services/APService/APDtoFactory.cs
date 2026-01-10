using Core.DTOs.APDto;
using Core.DTOs.SupplierDto;
using Core.Entities;
using Core.Interfaces.Services.IAPService;

namespace Infrastructure.Services.APService;

public class APDtoFactory : IAPDtoFactory
{
    public APComplexResDto APComplexRes(AccountsPayable accountsPayable)
    {
        return new APComplexResDto
        {
            Id = accountsPayable.Id,
            IssueDate = accountsPayable.IssueDate,
            DueDate = accountsPayable.DueDate,
            ExpectedPaymentDate = accountsPayable.ExpectedPaymentDate,
            Total = accountsPayable.Total,
            Balance = accountsPayable.Balance,
            Status = accountsPayable.Status,
            Notes = accountsPayable.Notes,
            Purchase = new APPurchaseResDto
            {
                Id = accountsPayable.Purchase!.Id,
                Sequential = accountsPayable.Purchase.Sequential,
                AccessKey = accountsPayable.Purchase.AccessKey,
                ReceiptType = accountsPayable.Purchase.ReceiptType,
                Status = accountsPayable.Purchase.Status,
                IsElectronic = accountsPayable.Purchase.IsElectronic,
                IssueDate = accountsPayable.Purchase.IssueDate,
                TotalPurchase = accountsPayable.Purchase.TotalPurchase
            },
            Supplier = new SupplierResDto
            {
                Id = accountsPayable.Supplier!.Id,
                BusinessName = accountsPayable.Supplier.BusinessName,
                Document = accountsPayable.Supplier.Document,
                Address = accountsPayable.Supplier.Address,
                Email = accountsPayable.Supplier.Email,
                Cellphone = accountsPayable.Supplier.Cellphone,
                Telephone = accountsPayable.Supplier.Telephone,
                IsActive = accountsPayable.Supplier.IsActive,
                CreatedAt = accountsPayable.Supplier.CreatedAt
            },
            Transactions = [.. accountsPayable.Transactions.Select(t => new APTransactionResDto
            {
                Id = t.Id,
                APTransactionType = t.APTransactionType,
                Amount = t.Amount,
                PaymentMethod = t.PaymentMethod,
                Reference = t.Reference,
                Notes = t.Notes,
                PaymentDetails = t.PaymentDetails,
                AccountsPayableId = t.AccountsPayableId,
                CreatedAt = t.CreatedAt
            })]
        };
    }

    public APSimpleResDto APSimpleRes(AccountsPayable accountsPayable)
    {
        return new APSimpleResDto
        {
            Id = accountsPayable.Id,
            IssueDate = accountsPayable.IssueDate,
            DueDate = accountsPayable.DueDate,
            ExpectedPaymentDate = accountsPayable.ExpectedPaymentDate,
            Total = accountsPayable.Total,
            Balance = accountsPayable.Balance,
            Status = accountsPayable.Status,
            Notes = accountsPayable.Notes,
            Purchase = new APPurchaseResDto
            {
                Id = accountsPayable.Purchase!.Id,
                Sequential = accountsPayable.Purchase.Sequential,
                AccessKey = accountsPayable.Purchase.AccessKey,
                ReceiptType = accountsPayable.Purchase.ReceiptType,
                Status = accountsPayable.Purchase.Status,
                IsElectronic = accountsPayable.Purchase.IsElectronic,
                IssueDate = accountsPayable.Purchase.IssueDate,
                TotalPurchase = accountsPayable.Purchase.TotalPurchase
            },
            Supplier = new SupplierResDto
            {
                Id = accountsPayable.Supplier!.Id,
                BusinessName = accountsPayable.Supplier.BusinessName,
                Document = accountsPayable.Supplier.Document,
                Address = accountsPayable.Supplier.Address,
                Email = accountsPayable.Supplier.Email,
                Cellphone = accountsPayable.Supplier.Cellphone,
                Telephone = accountsPayable.Supplier.Telephone,
                IsActive = accountsPayable.Supplier.IsActive,
                CreatedAt = accountsPayable.Supplier.CreatedAt
            }
        };
    }
}
