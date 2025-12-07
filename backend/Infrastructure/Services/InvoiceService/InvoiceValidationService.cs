using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Core.Entities;
using Core.Interfaces.Services.IInvoiceService;
using Core.Constants;

namespace Infrastructure.Services.InvoiceService;

public class InvoiceValidationService(StoreContext context) : IInvoiceValidationService
{
    public async Task<Business> ValidateBusinessAsync(int id)
    {
        var business = await context.Businesses.FirstOrDefaultAsync(b => b.Id == id) ??
        throw new Exception("Negocio no encontrado");

        return business;
    }

    public async Task<Customer> ValidateCustomerAsync(int id)
    {
        var customer = await context.Customers.FirstOrDefaultAsync(c => c.Id == id) ??
        throw new Exception("Cliente no encontrado");

        return customer;
    }

    public async Task<EmissionPoint> ValidateEmissionPointAsync(int id)
    {
        var emissionPoint = await context.EmissionPoints.FirstOrDefaultAsync(ep => ep.Id == id) ??
        throw new Exception("Punto de emisión no encontrado");

        return emissionPoint;
    }

    public async Task<Establishment> ValidateEstablishmentAsync(int id)
    {
        var establishment = await context.Establishments.FirstOrDefaultAsync(e => e.Id == id) ??
        throw new Exception("Establecimiento no encontrado");

        return establishment;
    }

    public void ValidateInvoiceIsDraft(Invoice invoice)
    {
        if (invoice.Status != InvoiceStatus.DRAFT)
        {
            throw new Exception("Solo las facturas en borrador pueden registrar pago");
        }
    }

    public async Task<User> ValidateUserAsync(int id)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id) ??
        throw new Exception("Usuario no encontrado");

        return user;
    }
}
