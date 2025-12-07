using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Core.Interfaces.Services.IInvoiceService;

namespace Infrastructure.Services.InvoiceService;

public class InvoiceSequentialService(StoreContext context) : IInvoiceSequentialService
{
    public async Task<string> GetNextSequentialAsync(int businessId, int establishmentId, int emissionPointId)
    {
        var last = await context.Invoices
        .Where(
            i =>
            i.BusinessId == businessId &&
            i.EstablishmentId == establishmentId &&
            i.EmissionPointId == emissionPointId)
        .OrderByDescending(i => i.Id)
        .FirstOrDefaultAsync();

        var nextNumber = 1;

        if (last != null && int.TryParse(last.Sequential, out var lastSeq))
        {
            nextNumber = lastSeq + 1;
        }

        return $"{nextNumber:D9}";
    }
}
