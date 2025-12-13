using Core.Entities;

namespace Core.Interfaces.Services.IInvoiceService;

public interface IInvoiceValidationService
{
    Task<Customer> ValidateCustomerAsync(int id);
    Task<Business> ValidateBusinessAsync(int id);
    Task<Establishment> ValidateEstablishmentAsync(int id);
    Task<EmissionPoint> ValidateEmissionPointAsync(int id);
    Task<User> ValidateUserAsync(int id);
    void ValidateInvoiceIsDraft(Invoice invoice);
}
