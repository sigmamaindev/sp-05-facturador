using Core.Interfaces.Services.IARService;
using Core.Entities;
using Core.DTOs.ARDto;
using Core.DTOs.CustomerDto;

namespace Infrastructure.Services.ARService;

public class ARDtoFactory : IARDtoFactory
{
    public ARComplexResDto ARComplexRes(AccountsReceivable accountsReceivable)
    {
        return new ARComplexResDto
        {
            Id = accountsReceivable.Id,
            IssueDate = accountsReceivable.IssueDate,
            DueDate = accountsReceivable.DueDate,
            ExpectedPaymentDate = accountsReceivable.ExpectedPaymentDate,
            PaymentMethod = accountsReceivable.Invoice!.PaymentMethod,
            OriginalAmount = accountsReceivable.OriginalAmount,
            Balance = accountsReceivable.Balance,
            Status = accountsReceivable.Status,
            Invoice = new ARInvoiceResDto
            {
                Id = accountsReceivable.Invoice!.Id,
                Sequential = accountsReceivable.Invoice.Sequential,
                EstablishmentCode = accountsReceivable.Invoice.Establishment!.Code,
                EmissionPointCode = accountsReceivable.Invoice.EmissionPoint!.Code,
                AccessKey = accountsReceivable.Invoice.AccessKey,
                AuthorizationNumber = accountsReceivable.Invoice.AuthorizationNumber,
                Environment = accountsReceivable.Invoice.Environment,
                ReceiptType = accountsReceivable.Invoice.ReceiptType,
                Status = accountsReceivable.Invoice.Status,
                IsElectronic = accountsReceivable.Invoice.IsElectronic,
                InvoiceDate = accountsReceivable.Invoice.InvoiceDate,
                DueDate = accountsReceivable.Invoice.DueDate,
                AuthorizationDate = accountsReceivable.Invoice.AuthorizationDate,
                PaymentMethod = accountsReceivable.Invoice.PaymentMethod,
                PaymentTermDays = accountsReceivable.Invoice.PaymentTermDays,
                TotalInvoice = accountsReceivable.Invoice.TotalInvoice
            },
            Customer = new CustomerResDto
            {
                Id = accountsReceivable.Customer!.Id,
                Document = accountsReceivable.Customer.Document,
                DocumentType = accountsReceivable.Customer.DocumentType,
                Name = accountsReceivable.Customer.Name,
                Email = accountsReceivable.Customer.Email,
                Address = accountsReceivable.Customer.Address,
                Cellphone = accountsReceivable.Customer.Cellphone,
                Telephone = accountsReceivable.Customer.Telephone,
                IsActive = accountsReceivable.Customer.IsActive,
                CreatedAt = accountsReceivable.Customer.CreatedAt
            },
            Transactions = [.. accountsReceivable.Transactions.Select(t => new ARTransactionResDto {
                   Id = t.Id,
                   ARTransactionType = t.ARTransactionType,
                   Amount = t.Amount,
                   PaymentMethod = t.PaymentMethod,
                   Reference = t.Reference,
                   Notes = t.Notes,
                   PaymentDetails = t.PaymentDetails,
                   AccountReceivableId = t.AccountReceivableId,
                   CreatedAt = t.CreatedAt
            })],
        };
    }

    public ARSimpleResDto ARSimpleRes(AccountsReceivable accountsReceivable)
    {
        return new ARSimpleResDto
        {
            Id = accountsReceivable.Id,
            IssueDate = accountsReceivable.IssueDate,
            DueDate = accountsReceivable.DueDate,
            ExpectedPaymentDate = accountsReceivable.ExpectedPaymentDate,
            PaymentMethod = accountsReceivable.Invoice!.PaymentMethod,
            OriginalAmount = accountsReceivable.OriginalAmount,
            Balance = accountsReceivable.Balance,
            Status = accountsReceivable.Status,
            Invoice = new ARInvoiceResDto
            {
                Id = accountsReceivable.Invoice!.Id,
                Sequential = accountsReceivable.Invoice.Sequential,
                EstablishmentCode = accountsReceivable.Invoice.Establishment!.Code,
                EmissionPointCode = accountsReceivable.Invoice.EmissionPoint!.Code,
                AccessKey = accountsReceivable.Invoice.AccessKey,
                AuthorizationNumber = accountsReceivable.Invoice.AuthorizationNumber,
                Environment = accountsReceivable.Invoice.Environment,
                ReceiptType = accountsReceivable.Invoice.ReceiptType,
                Status = accountsReceivable.Invoice.Status,
                IsElectronic = accountsReceivable.Invoice.IsElectronic,
                InvoiceDate = accountsReceivable.Invoice.InvoiceDate,
                DueDate = accountsReceivable.Invoice.DueDate,
                AuthorizationDate = accountsReceivable.Invoice.AuthorizationDate,
                PaymentMethod = accountsReceivable.Invoice.PaymentMethod,
                PaymentTermDays = accountsReceivable.Invoice.PaymentTermDays,
                TotalInvoice = accountsReceivable.Invoice.TotalInvoice
            },
            Customer = new CustomerResDto
            {
                Id = accountsReceivable.Customer!.Id,
                Document = accountsReceivable.Customer.Document,
                DocumentType = accountsReceivable.Customer.DocumentType,
                Name = accountsReceivable.Customer.Name,
                Email = accountsReceivable.Customer.Email,
                Address = accountsReceivable.Customer.Address,
                Cellphone = accountsReceivable.Customer.Cellphone,
                Telephone = accountsReceivable.Customer.Telephone,
                IsActive = accountsReceivable.Customer.IsActive,
                CreatedAt = accountsReceivable.Customer.CreatedAt
            },
        };
    }
}
