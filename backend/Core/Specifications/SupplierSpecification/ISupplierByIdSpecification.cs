using Core.Entities;
using System.Linq;
using System.Linq.Expressions;

namespace Core.Interfaces.Specifications.SupplierSpecification;

public interface ISupplierByIdSpecification
{
    int SupplierId { get; }
    Expression<Func<Supplier, bool>> Criteria { get; }
    IQueryable<Supplier> Apply(IQueryable<Supplier> query);
}
