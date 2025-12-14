using Core.Entities;

namespace Core.Interfaces.Services.IPurchaseService;

public interface IPurchaseValidationService
{
    Task<Supplier> ValidateSupplierAsync(int id);
    Task<Business> ValidateBusinessAsync(int id);
    Task<Establishment> ValidateEstablishmentAsync(int id);
    Task<EmissionPoint> ValidateEmissionPointAsync(int id);
    Task<User> ValidateUserAsync(int id);
    void ValidatePurchaseIsDraft(Purchase purchase);
}
