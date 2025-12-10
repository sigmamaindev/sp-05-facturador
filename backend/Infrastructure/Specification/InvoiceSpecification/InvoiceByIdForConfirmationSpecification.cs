using System.Linq;
using Core.Entities;
using Core.Interfaces.Specifications.InvoiceSpecification;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Specification.InvoiceSpecification;

public class InvoiceByIdForConfirmationSpecification : IInvoiceByIdForConfirmationSpecification
{
    public InvoiceByIdForConfirmationSpecification(int invoiceId)
    {
        InvoiceId = invoiceId;
        Criteria = invoice => invoice.Id == invoiceId;
    }

    public int InvoiceId { get; }

    public IQueryable<Invoice> Apply(IQueryable<Invoice> query)
    {
        return query
            .Where(Criteria)
            .Include(i => i.Customer)
            .Include(i => i.Business)
            .Include(i => i.Establishment)
            .Include(i => i.EmissionPoint)
            .Include(i => i.User)
            .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Product)
                    .ThenInclude(p => p!.Tax)
            .Include(i => i.InvoiceDetails)
                .ThenInclude(d => d.Warehouse);
    }

    public System.Linq.Expressions.Expression<Func<Invoice, bool>> Criteria { get; }
}
