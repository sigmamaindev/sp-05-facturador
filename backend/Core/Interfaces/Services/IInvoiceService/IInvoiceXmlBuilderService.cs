using Core.Entities;

namespace Core.Interfaces.Services.IInvoiceService;

public interface IInvoiceXmlBuilderService
{
    string BuildXMLInvoice(Invoice invoice, Business business, Establishment establishment, EmissionPoint emissionPoint, Customer customer);
}
