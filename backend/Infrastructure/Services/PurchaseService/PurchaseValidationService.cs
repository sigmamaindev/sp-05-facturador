using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Core.Constants;
using Core.Entities;
using Core.Interfaces.Services.IPurchaseService;

namespace Infrastructure.Services.PurchaseService;

public class PurchaseValidationService(StoreContext context) : IPurchaseValidationService
{
    public async Task<Business> ValidateBusinessAsync(int id)
    {
        var business = await context.Businesses.FirstOrDefaultAsync(b => b.Id == id) ??
        throw new Exception("Negocio no encontrado");

        return business;
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

    public async Task<Supplier> ValidateSupplierAsync(int id)
    {
        var supplier = await context.Suppliers.FirstOrDefaultAsync(s => s.Id == id) ??
        throw new Exception("Proveedor no encontrado");

        return supplier;
    }

    public void ValidatePurchaseIsDraft(Purchase purchase)
    {
        if (purchase.Status != PurchaseStatus.DRAFT)
        {
            throw new Exception("Solo las compras en borrador pueden registrar pago");
        }
    }

    public async Task<User> ValidateUserAsync(int id)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == id) ??
        throw new Exception("Usuario no encontrado");

        return user;
    }
}
