using System.Linq;
using Core.Entities;
using Core.Interfaces.Specifications.SupplierSpecification;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Specification.SupplierSpecification;

public class SupplierByIdSpecification : ISupplierByIdSpecification
{
    public SupplierByIdSpecification(int supplierId)
    {
        SupplierId = supplierId;
        Criteria = supplier => supplier.Id == supplierId;
    }

    public int SupplierId { get; }

    public IQueryable<Supplier> Apply(IQueryable<Supplier> query)
    {
        return query
            .Where(Criteria)
            .Include(s => s.Business)
            .Include(s => s.Purchases);
    }

    public System.Linq.Expressions.Expression<Func<Supplier, bool>> Criteria { get; }
}
