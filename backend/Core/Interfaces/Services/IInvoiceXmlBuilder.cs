using Core.Entities;

namespace Core.Interfaces.Services;

public interface IInvoiceXmlBuilder
{
    string BuildXMLInvoice(Invoice invoice, Business business, Establishment establishment, EmissionPoint emissionPoint, Customer customer);
}
