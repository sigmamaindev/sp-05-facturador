using Core.Entities;
using System.Linq;
using System.Linq.Expressions;

namespace Core.Interfaces.Specifications.ProductWarehouseSpecification;

public interface IProductWarehouseByProductAndWarehouseSpecification
{
    int ProductId { get; }
    int WarehouseId { get; }
    Expression<Func<ProductWarehouse, bool>> Criteria { get; }
    IQueryable<ProductWarehouse> Apply(IQueryable<ProductWarehouse> query);
}
