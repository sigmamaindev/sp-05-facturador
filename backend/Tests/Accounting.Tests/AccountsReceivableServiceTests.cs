using Core.DTOs.AccountingDto;
using Core.Entities;
using Core.Interfaces.Services.IUtilService;
using Infrastructure.Data;
using Infrastructure.Services.AccountingService;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Accounting.Tests;

public class AccountsReceivableServiceTests
{
    [Fact]
    public async Task CreateFromInvoice_IsIdempotent()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);

        var invoice = await SeedInvoiceAsync(context, businessId: 1, customerId: 1, total: 100m);

        var service = new AccountsReceivableService(context, new FakeUserContextService
        {
            BusinessId = 1,
            UserId = 1
        });

        var first = await service.CreateFromInvoiceAsync(invoice.Id);
        var second = await service.CreateFromInvoiceAsync(invoice.Id);

        Assert.Equal(first.Id, second.Id);
        Assert.Equal(1, await context.AccountsReceivables.CountAsync());
        Assert.Equal(100m, first.Balance);
        Assert.Single(first.Transactions);
    }

    [Fact]
    public async Task Payment_CannotExceed_CurrentBalance()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);

        var invoice = await SeedInvoiceAsync(context, businessId: 1, customerId: 1, total: 100m);

        var service = new AccountsReceivableService(context, new FakeUserContextService
        {
            BusinessId = 1,
            UserId = 1
        });

        var ar = await service.CreateFromInvoiceAsync(invoice.Id);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.RegisterARPaymentAsync(ar.Id, new PaymentReqDto
            {
                Date = DateTime.UtcNow,
                Amount = 101m,
                MethodCode = "CASH"
            }));

        var reloaded = await service.GetARByIdAsync(ar.Id);
        Assert.Equal(100m, reloaded.Balance);
        Assert.Equal(AccountStatus.Open, reloaded.Status);
        Assert.Single(reloaded.Transactions);
    }

    [Fact]
    public async Task Balance_And_Status_Update_AfterPartialPayments()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var context = CreateContext(dbName);

        var invoice = await SeedInvoiceAsync(context, businessId: 1, customerId: 1, total: 100m);

        var service = new AccountsReceivableService(context, new FakeUserContextService
        {
            BusinessId = 1,
            UserId = 1
        });

        var ar = await service.CreateFromInvoiceAsync(invoice.Id);

        var afterFirstPayment = await service.RegisterARPaymentAsync(ar.Id, new PaymentReqDto
        {
            Date = DateTime.UtcNow,
            Amount = 30m,
            MethodCode = "CASH"
        });

        Assert.Equal(70m, afterFirstPayment.Balance);
        Assert.Equal(AccountStatus.PartiallyPaid, afterFirstPayment.Status);

        var afterSecondPayment = await service.RegisterARPaymentAsync(ar.Id, new PaymentReqDto
        {
            Date = DateTime.UtcNow,
            Amount = 70m,
            MethodCode = "CASH"
        });

        Assert.Equal(0m, afterSecondPayment.Balance);
        Assert.Equal(AccountStatus.Paid, afterSecondPayment.Status);
    }

    private static StoreContext CreateContext(string databaseName)
    {
        var options = new DbContextOptionsBuilder<StoreContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

        return new StoreContext(options);
    }

    private static async Task<Invoice> SeedInvoiceAsync(StoreContext context, int businessId, int customerId, decimal total)
    {
        var business = new Business
        {
            Id = businessId,
            Document = "9999999999",
            Name = "Test Business",
            Address = "Test Address"
        };

        var customer = new Customer
        {
            Id = customerId,
            Document = "0102030405",
            DocumentType = "RUC",
            Name = "Test Customer",
            Email = "test@example.com",
            Address = "Test Address",
            BusinessId = businessId
        };

        var invoice = new Invoice
        {
            Sequential = "001-001-000000001",
            PaymentMethod = "CASH",
            InvoiceDate = DateTime.UtcNow.Date,
            PaymentTermDays = 0,
            DueDate = DateTime.UtcNow.Date,
            TotalInvoice = total,
            CustomerId = customerId,
            BusinessId = businessId,
            UserId = 1,
            EstablishmentId = 1,
            EmissionPointId = 1
        };

        context.Businesses.Add(business);
        context.Customers.Add(customer);
        context.Invoices.Add(invoice);

        await context.SaveChangesAsync();

        return invoice;
    }

    private sealed class FakeUserContextService : IUserContextService
    {
        public int BusinessId { get; init; }
        public int EstablishmentId { get; init; }
        public int EmissionPointId { get; init; }
        public int UserId { get; init; }
    }
}

