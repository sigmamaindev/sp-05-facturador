using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Core.Constants;
using Core.Entities;
using Core.DTOs.InvoiceDto;
using Core.Interfaces.Services.IInvoiceService;

namespace Infrastructure.Services.InvoiceService;

public class InvoiceEditionService(StoreContext context) : IInvoiceEditionService
{
    public async Task AddInvoiceDetailsAsync(Invoice invoice, IEnumerable<InvoiceDetailCreateReqDto> details)
    {
        if (invoice.BusinessId <= 0)
        {
            throw new InvalidOperationException("Factura inválida");
        }

        var inputs = details
            .Select(d => new InvoiceLineInput(
                d.ProductId,
                d.UnitMeasureId,
                d.WarehouseId,
                d.Quantity,
                d.NetWeight,
                d.GrossWeight,
                d.UnitPrice,
                d.Discount))
            .ToList();

        if (inputs.Count == 0)
        {
            return;
        }

        var resolvedLines = await ResolveLinesAsync(invoice.BusinessId, inputs);

        foreach (var resolved in resolvedLines)
        {
            invoice.InvoiceDetails.Add(new InvoiceDetail
            {
                InvoiceId = invoice.Id,
                ProductId = resolved.Product.Id,
                Product = resolved.Product,
                ProductPresentationId = resolved.Presentation.Id,
                ProductPresentation = resolved.Presentation,
                WarehouseId = resolved.Warehouse.Id,
                Warehouse = resolved.Warehouse,
                UnitMeasureId = resolved.UnitMeasure.Id,
                UnitMeasure = resolved.UnitMeasure,
                Quantity = resolved.Quantity,
                NetWeight = resolved.NetWeight,
                GrossWeight = resolved.GrossWeight,
                UnitPrice = resolved.UnitPrice,
                PriceLevel = resolved.PriceLevel,
                Discount = resolved.Discount,
                TaxId = resolved.Tax.Id,
                Tax = resolved.Tax,
                TaxRate = resolved.Tax.Rate
            });
        }
    }

    public Invoice BuildInvoice(InvoiceCreateReqDto dto, Customer customer, Business business, Establishment establishment, EmissionPoint emissionPoint, User user, string sequential, DateTime invoiceDate)
    {
        return new Invoice
        {
            ReceiptType = dto.ReceiptType,
            Environment = dto.Environment,
            IsElectronic = dto.IsElectronic,
            InvoiceDate = invoiceDate,
            DueDate = invoiceDate,
            CustomerId = customer.Id,
            BusinessId = business.Id,
            EstablishmentId = establishment.Id,
            EmissionPointId = emissionPoint.Id,
            UserId = user.Id,
            Sequential = sequential,
            PaymentMethod = dto.PaymentMethod,
            PaymentTermDays = dto.PaymentTermDays,
            Description = dto.Description,
            AdditionalInformation = dto.AdditionalInformation,
            Status = InvoiceStatus.DRAFT,
            Business = business,
            Establishment = establishment,
            EmissionPoint = emissionPoint,
            Customer = customer,
            InvoiceDetails = []
        };
    }

    public async Task<Invoice?> CheckInvoiceExistenceAsync(int invoiceId, int businessId, int establishmentId, int emissionPointId, int userId)
    {
        return await context.Invoices
        .Include(i => i.Customer)
        .Include(i => i.Business)
        .Include(i => i.Establishment)
        .Include(i => i.EmissionPoint)
        .Include(i => i.User)
        .Include(i => i.InvoiceDetails)
        .ThenInclude(d => d.Product)
        .Include(i => i.InvoiceDetails)
        .ThenInclude(d => d.ProductPresentation)
        .ThenInclude(pp => pp!.UnitMeasure)
        .Include(i => i.InvoiceDetails)
        .ThenInclude(d => d.UnitMeasure)
        .Include(i => i.InvoiceDetails)
        .ThenInclude(d => d.Warehouse)
        .Include(i => i.InvoiceDetails)
        .ThenInclude(d => d.Tax)
        .FirstOrDefaultAsync(
            i =>
            i.Id == invoiceId &&
            i.BusinessId == businessId &&
            i.EstablishmentId == establishmentId &&
            i.EmissionPointId == emissionPointId &&
            i.UserId == userId);
    }

    public async Task UpsertInvoiceAsync(Invoice invoice, InvoiceUpdateReqDto dto, IEnumerable<InvoiceDetailUpdateReqDto> details)
    {
        if (invoice.BusinessId <= 0)
        {
            throw new InvalidOperationException("Factura inválida");
        }

        var inputs = details
            .Select(d => new InvoiceLineInput(
                d.ProductId,
                d.UnitMeasureId,
                d.WarehouseId,
                d.Quantity,
                d.NetWeight,
                d.GrossWeight,
                d.UnitPrice,
                d.Discount))
            .ToList();

        var resolvedLines = await ResolveLinesAsync(invoice.BusinessId, inputs);

        var incomingKeys = resolvedLines
            .Select(r => (r.Product.Id, r.Warehouse.Id, r.UnitMeasure.Id))
            .ToHashSet();

        foreach (var oldDetail in invoice.InvoiceDetails.ToList())
        {
            var oldKey = (oldDetail.ProductId, oldDetail.WarehouseId ?? 0, oldDetail.UnitMeasureId);

            if (!incomingKeys.Contains(oldKey))
            {
                context.InvoiceDetails.Remove(oldDetail);
                invoice.InvoiceDetails.Remove(oldDetail);
            }
        }

        foreach (var resolved in resolvedLines)
        {
            var existingDetail = invoice.InvoiceDetails.FirstOrDefault(
                d =>
                    d.ProductId == resolved.Product.Id &&
                    d.WarehouseId == resolved.Warehouse.Id &&
                    d.UnitMeasureId == resolved.UnitMeasure.Id);

            if (existingDetail != null)
            {
                existingDetail.ProductId = resolved.Product.Id;
                existingDetail.Product = resolved.Product;
                existingDetail.ProductPresentationId = resolved.Presentation.Id;
                existingDetail.ProductPresentation = resolved.Presentation;
                existingDetail.WarehouseId = resolved.Warehouse.Id;
                existingDetail.Warehouse = resolved.Warehouse;
                existingDetail.UnitMeasureId = resolved.UnitMeasure.Id;
                existingDetail.UnitMeasure = resolved.UnitMeasure;
                existingDetail.Quantity = resolved.Quantity;
                existingDetail.NetWeight = resolved.NetWeight;
                existingDetail.GrossWeight = resolved.GrossWeight;
                existingDetail.UnitPrice = resolved.UnitPrice;
                existingDetail.PriceLevel = resolved.PriceLevel;
                existingDetail.Discount = resolved.Discount;
                existingDetail.TaxId = resolved.Tax.Id;
                existingDetail.Tax = resolved.Tax;
                existingDetail.TaxRate = resolved.Tax.Rate;
            }
            else
            {
                invoice.InvoiceDetails.Add(new InvoiceDetail
                {
                    InvoiceId = invoice.Id,
                    ProductId = resolved.Product.Id,
                    Product = resolved.Product,
                    ProductPresentationId = resolved.Presentation.Id,
                    ProductPresentation = resolved.Presentation,
                    WarehouseId = resolved.Warehouse.Id,
                    Warehouse = resolved.Warehouse,
                    UnitMeasureId = resolved.UnitMeasure.Id,
                    UnitMeasure = resolved.UnitMeasure,
                    Quantity = resolved.Quantity,
                    NetWeight = resolved.NetWeight,
                    GrossWeight = resolved.GrossWeight,
                    UnitPrice = resolved.UnitPrice,
                    PriceLevel = resolved.PriceLevel,
                    Discount = resolved.Discount,
                    TaxId = resolved.Tax.Id,
                    Tax = resolved.Tax,
                    TaxRate = resolved.Tax.Rate
                });
            }
        }
    }

    private sealed record InvoiceLineInput(
        int ProductId,
        int UnitMeasureId,
        int WarehouseId,
        decimal Quantity,
        decimal NetWeight,
        decimal GrossWeight,
        decimal UnitPrice,
        decimal Discount);

    private sealed record ResolvedInvoiceLine(
        Product Product,
        ProductPresentation Presentation,
        UnitMeasure UnitMeasure,
        Warehouse Warehouse,
        Tax Tax,
        decimal Quantity,
        decimal UnitPrice,
        int PriceLevel,
        decimal Discount,
        decimal NetWeight,
        decimal GrossWeight);

    private async Task<List<ResolvedInvoiceLine>> ResolveLinesAsync(int businessId, IReadOnlyList<InvoiceLineInput> inputs)
    {
        if (inputs.Count == 0)
        {
            return [];
        }

        var productIds = inputs.Select(i => i.ProductId).Distinct().ToList();
        var warehouseIds = inputs.Select(i => i.WarehouseId).Distinct().ToList();
        var unitMeasureIds = inputs.Where(i => i.UnitMeasureId > 0).Select(i => i.UnitMeasureId).Distinct().ToList();

        var products = await context.Products
            .Include(p => p.Tax)
            .Where(p => productIds.Contains(p.Id) && p.BusinessId == businessId && p.IsActive)
            .ToListAsync();

        var productById = products.ToDictionary(p => p.Id);

        foreach (var productId in productIds)
        {
            if (!productById.ContainsKey(productId))
            {
                throw new InvalidOperationException($"Producto {productId} no encontrado");
            }
        }

        var warehouses = await context.Warehouses
            .Where(w => warehouseIds.Contains(w.Id) && w.BusinessId == businessId && w.IsActive)
            .ToListAsync();

        var warehouseById = warehouses.ToDictionary(w => w.Id);

        foreach (var warehouseId in warehouseIds)
        {
            if (!warehouseById.ContainsKey(warehouseId))
            {
                throw new InvalidOperationException($"Bodega {warehouseId} no encontrada");
            }
        }

        var presentations = await context.Set<ProductPresentation>()
            .Include(pp => pp.UnitMeasure)
            .Where(pp =>
                productIds.Contains(pp.ProductId) &&
                pp.IsActive &&
                (pp.IsDefault || unitMeasureIds.Contains(pp.UnitMeasureId)))
            .ToListAsync();

        var presentationByKey = presentations.ToDictionary(pp => (pp.ProductId, pp.UnitMeasureId));
        var defaultPresentationByProduct = presentations
            .Where(pp => pp.IsDefault)
            .ToDictionary(pp => pp.ProductId);

        var goodsLinePairs = new HashSet<(int ProductId, int WarehouseId)>();
        foreach (var input in inputs)
        {
            if (input.Quantity <= 0)
            {
                throw new InvalidOperationException($"Cantidad inválida para el producto {input.ProductId}");
            }

            if (input.WarehouseId <= 0)
            {
                throw new InvalidOperationException($"Bodega inválida para el producto {input.ProductId}");
            }

            if (input.NetWeight < 0 || input.GrossWeight < 0)
            {
                throw new InvalidOperationException($"Pesos inválidos para el producto {input.ProductId}");
            }

            if (input.NetWeight > 0 && input.GrossWeight > 0 && input.GrossWeight < input.NetWeight)
            {
                throw new InvalidOperationException($"El peso bruto no puede ser menor al peso neto para el producto {input.ProductId}");
            }

            var product = productById[input.ProductId];
            if (product.Type != ProductTypes.SERVICE)
            {
                goodsLinePairs.Add((product.Id, input.WarehouseId));
            }
        }

        var goodsProductIds = goodsLinePairs.Select(p => p.ProductId).Distinct().ToList();
        var goodsWarehouseIds = goodsLinePairs.Select(p => p.WarehouseId).Distinct().ToList();

        var stocks = goodsLinePairs.Count == 0
            ? []
            : await context.ProductWarehouses
                .Where(pw => goodsProductIds.Contains(pw.ProductId) && goodsWarehouseIds.Contains(pw.WarehouseId))
                .ToListAsync();

        var stockByKey = stocks.ToDictionary(pw => (pw.ProductId, pw.WarehouseId));

        var resolved = new List<ResolvedInvoiceLine>(inputs.Count);

        foreach (var input in inputs)
        {
            var product = productById[input.ProductId];
            var warehouse = warehouseById[input.WarehouseId];

            var presentation = ResolvePresentation(
                product,
                input.UnitMeasureId,
                presentationByKey,
                defaultPresentationByProduct);

            var unitMeasure = presentation.UnitMeasure
                ?? throw new InvalidOperationException($"Unidad de medida {presentation.UnitMeasureId} no encontrada para el producto {product.Name}");

            if (unitMeasure.BusinessId != businessId || !unitMeasure.IsActive)
            {
                throw new InvalidOperationException($"Unidad de medida {unitMeasure.Id} inválida para el negocio actual");
            }

            if (unitMeasure.FactorBase <= 0)
            {
                throw new InvalidOperationException($"Factor de unidad inválido para el producto {product.Name}");
            }

            var tax = product.Tax
                ?? throw new InvalidOperationException($"Impuesto {product.TaxId} no encontrado para el producto {product.Name}");

            if (tax.BusinessId != businessId || !tax.IsActive)
            {
                throw new InvalidOperationException($"Impuesto {tax.Id} inválido para el negocio actual");
            }

            if (product.Type != ProductTypes.SERVICE)
            {
                if (!stockByKey.TryGetValue((product.Id, warehouse.Id), out var stock))
                {
                    throw new InvalidOperationException($"El producto {product.Name} no tiene stock configurado en la bodega seleccionada");
                }

                var quantityBase = input.Quantity * unitMeasure.FactorBase;

                if (stock.Stock < quantityBase)
                {
                    throw new InvalidOperationException($"Stock insuficiente para el producto {product.Name}.");
                }
            }

            var (unitPrice, priceLevel) = ResolvePrice(presentation, input.UnitPrice);

            var netWeight = input.NetWeight;
            var grossWeight = input.GrossWeight;

            if (netWeight <= 0 && grossWeight > 0)
            {
                netWeight = grossWeight;
            }
            else if (grossWeight <= 0 && netWeight > 0)
            {
                grossWeight = netWeight;
            }
            else if (netWeight <= 0 && grossWeight <= 0)
            {
                netWeight = input.Quantity * presentation.NetWeight;
                grossWeight = input.Quantity * presentation.GrossWeight;
            }

            resolved.Add(new ResolvedInvoiceLine(
                product,
                presentation,
                unitMeasure,
                warehouse,
                tax,
                input.Quantity,
                unitPrice,
                priceLevel,
                input.Discount,
                netWeight,
                grossWeight));
        }

        var duplicateKeys = resolved
            .GroupBy(r => (r.Product.Id, r.Warehouse.Id, r.UnitMeasure.Id))
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .FirstOrDefault();

        if (duplicateKeys != default)
        {
            throw new InvalidOperationException("Detalle duplicado para el mismo producto/bodega/unidad de medida");
        }

        return resolved;
    }

    private static ProductPresentation ResolvePresentation(
        Product product,
        int unitMeasureId,
        IReadOnlyDictionary<(int ProductId, int UnitMeasureId), ProductPresentation> presentationByKey,
        IReadOnlyDictionary<int, ProductPresentation> defaultPresentationByProduct)
    {
        if (unitMeasureId > 0)
        {
            if (!presentationByKey.TryGetValue((product.Id, unitMeasureId), out var presentation))
            {
                throw new InvalidOperationException($"El producto {product.Name} no tiene la presentación configurada para la unidad de medida {unitMeasureId}");
            }

            return presentation;
        }

        if (!defaultPresentationByProduct.TryGetValue(product.Id, out var defaultPresentation))
        {
            throw new InvalidOperationException($"El producto {product.Name} no tiene una presentación default activa");
        }

        return defaultPresentation;
    }

    private static (decimal UnitPrice, int PriceLevel) ResolvePrice(ProductPresentation presentation, decimal requestedUnitPrice)
    {
        if (requestedUnitPrice > 0)
        {
            if (requestedUnitPrice == presentation.Price01) return (requestedUnitPrice, 1);
            if (requestedUnitPrice == presentation.Price02) return (requestedUnitPrice, 2);
            if (requestedUnitPrice == presentation.Price03) return (requestedUnitPrice, 3);
            if (requestedUnitPrice == presentation.Price04) return (requestedUnitPrice, 4);
            return (requestedUnitPrice, 0);
        }

        return (presentation.Price01, 1);
    }
}
