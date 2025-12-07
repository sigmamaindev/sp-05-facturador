namespace Core.Interfaces.Services.IInvoiceService;

public interface IInvoiceSequentialService
{
    Task<string> GetNextSequentialAsync(int businessId, int establishmentId, int emissionPointId);
}
