using System.Linq;
using Core.Entities;
using Core.Interfaces.Specifications.PurchaseSpecification;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Specification.PurchaseSpecification;

public class PurchaseByIdWithDetailsSpecification : IPurchaseByIdWithDetailsSpecification
{
    public PurchaseByIdWithDetailsSpecification(int purchaseId)
    {
        PurchaseId = purchaseId;
        Criteria = purchase => purchase.Id == purchaseId;
    }

    public int PurchaseId { get; }

    public IQueryable<Purchase> Apply(IQueryable<Purchase> query)
    {
        return query
            .Where(Criteria)
            .Include(p => p.Supplier)
            .Include(p => p.Business)
            .Include(p => p.Establishment)
            .Include(p => p.Warehouse)
            .Include(p => p.PurchaseDetails)
                .ThenInclude(d => d.Product)
            .Include(p => p.PurchaseDetails)
                .ThenInclude(d => d.Tax);
    }

    public System.Linq.Expressions.Expression<Func<Purchase, bool>> Criteria { get; }
}
