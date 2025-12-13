using Core.DTOs.InvoiceDto;
using Core.Entities;

namespace Core.Interfaces.Services.IInvoiceService;

public interface IInvoiceEditionService
{
    Task<Invoice?> CheckInvoiceExistenceAsync(int invoiceId, int businessId, int establishmentId, int emissionPointId, int userId);
    Invoice BuildInvoice(InvoiceCreateReqDto dto, Customer customer, Business business, Establishment establishment, EmissionPoint emissionPoint, User user, string sequential, DateTime invoiceDate);
    Task AddInvoiceDetailsAsync(Invoice invoice, IEnumerable<InvoiceDetailCreateReqDto> details);
    Task UpsertInvoiceAsync(Invoice invoice, InvoiceUpdateReqDto dto, IEnumerable<InvoiceDetailUpdateReqDto> details);
}
