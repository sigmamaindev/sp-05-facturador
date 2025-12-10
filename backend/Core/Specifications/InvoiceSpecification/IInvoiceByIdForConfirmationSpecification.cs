using Core.Entities;
using System.Linq;
using System.Linq.Expressions;

namespace Core.Interfaces.Specifications.InvoiceSpecification;

public interface IInvoiceByIdForConfirmationSpecification
{
    int InvoiceId { get; }
    Expression<Func<Invoice, bool>> Criteria { get; }
    IQueryable<Invoice> Apply(IQueryable<Invoice> query);
}
