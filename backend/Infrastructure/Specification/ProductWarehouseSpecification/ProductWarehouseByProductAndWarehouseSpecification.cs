using System.Linq;
using Core.Entities;
using Core.Interfaces.Specifications.ProductWarehouseSpecification;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Specification.ProductWarehouseSpecification;

public class ProductWarehouseByProductAndWarehouseSpecification : IProductWarehouseByProductAndWarehouseSpecification
{
    public ProductWarehouseByProductAndWarehouseSpecification(int productId, int warehouseId)
    {
        ProductId = productId;
        WarehouseId = warehouseId;
        Criteria = pw => pw.ProductId == productId && pw.WarehouseId == warehouseId;
    }

    public int ProductId { get; }
    public int WarehouseId { get; }

    public IQueryable<ProductWarehouse> Apply(IQueryable<ProductWarehouse> query)
    {
        return query
            .Where(Criteria)
            .Include(pw => pw.Product)!
                .ThenInclude(p => p.Tax)
            .Include(pw => pw.Warehouse);
    }

    public System.Linq.Expressions.Expression<Func<ProductWarehouse, bool>> Criteria { get; }
}
