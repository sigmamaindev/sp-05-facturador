using Core.Entities;
using System.Linq;
using System.Linq.Expressions;

namespace Core.Interfaces.Specifications.PurchaseSpecification;

public interface IPurchaseByIdWithDetailsSpecification
{
    int PurchaseId { get; }
    Expression<Func<Purchase, bool>> Criteria { get; }
    IQueryable<Purchase> Apply(IQueryable<Purchase> query);
}
