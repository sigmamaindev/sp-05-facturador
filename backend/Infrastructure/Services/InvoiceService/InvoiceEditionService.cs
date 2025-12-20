using Microsoft.EntityFrameworkCore;
using Infrastructure.Data;
using Core.Constants;
using Core.Entities;
using Core.DTOs.InvoiceDto;
using Core.Interfaces.Services.IInvoiceService;

namespace Infrastructure.Services.InvoiceService;

public class InvoiceEditionService(StoreContext context, IInvoiceStockService stock) : IInvoiceEditionService
{
    public async Task AddInvoiceDetailsAsync(Invoice invoice, IEnumerable<InvoiceDetailCreateReqDto> details)
    {
        foreach (var detail in details)
        {
            var product = await context.Products
            .Include(p => p.Tax)
            .FirstOrDefaultAsync(p => p.Id == detail.ProductId)
            ?? throw new InvalidOperationException($"Producto {detail.ProductId} no encontrado");

            var warehouse = await context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == detail.WarehouseId)
            ?? throw new InvalidOperationException($"Bodega {detail.WarehouseId} no encontrada");

            var unitMeasureId = detail.UnitMeasureId > 0
                ? detail.UnitMeasureId
                : product.UnitMeasureId;

            var unitMeasure = await context.UnitMeasures
            .FirstOrDefaultAsync(
                um =>
                um.Id == unitMeasureId &&
                um.BusinessId == invoice.BusinessId)
            ?? throw new InvalidOperationException($"Unidad de medida {unitMeasureId} no encontrada para el negocio actual");

            await stock.ReserveStockAsync(
                detail.ProductId,
                detail.WarehouseId,
                detail.Quantity
            );

            var netWeight = detail.Quantity;
            var grossWeight = netWeight;

            var subtotal = detail.Quantity * product.Price;
            var taxableBase = subtotal - detail.Discount;
            var taxRate = product.Tax?.Rate ?? 0;
            var taxValue = taxableBase * (taxRate / 100);
            var total = taxableBase + taxValue;

            invoice.InvoiceDetails.Add(new InvoiceDetail
            {
                InvoiceId = invoice.Id,
                ProductId = product.Id,
                WarehouseId = warehouse.Id,
                UnitMeasureId = unitMeasure.Id,
                NetWeight = netWeight,
                GrossWeight = grossWeight,
                Quantity = detail.Quantity,
                UnitPrice = product.Price,
                Discount = detail.Discount,
                Subtotal = taxableBase,
                TaxId = product.TaxId,
                TaxRate = taxRate,
                TaxValue = taxValue,
                Total = total
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
        .ThenInclude(p => p!.UnitMeasure)
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
        var incomingKeys = dto.Details
            .Select(d => (d.ProductId, d.WarehouseId))
            .ToHashSet();

        foreach (var oldDetail in invoice.InvoiceDetails.ToList())
        {
            var key = (oldDetail.ProductId, oldDetail.WarehouseId);

            if (!incomingKeys.Contains(key))
            {
                await stock.ReturnStockAsync(oldDetail.ProductId, oldDetail.WarehouseId, oldDetail.Quantity);

                context.InvoiceDetails.Remove(oldDetail);

                invoice.InvoiceDetails.Remove(oldDetail);
            }
        }

        foreach (var detail in details)
        {
            var existingDetail = invoice.InvoiceDetails
            .FirstOrDefault(
                d =>
                d.ProductId == detail.ProductId &&
                d.WarehouseId == detail.WarehouseId);

            var product = await context.Products
            .Include(p => p.Tax)
            .FirstOrDefaultAsync(
                p =>
                p.Id == detail.ProductId) ??
            throw new Exception($"Producto {detail.ProductId} no encontrado");

            var unitMeasureId = detail.UnitMeasureId > 0
                ? detail.UnitMeasureId
                : product.UnitMeasureId;

            var unitMeasure = await context.UnitMeasures
            .FirstOrDefaultAsync(
                um =>
                um.Id == unitMeasureId &&
                um.BusinessId == invoice.BusinessId)
            ?? throw new InvalidOperationException($"Unidad de medida {unitMeasureId} no encontrada para el negocio actual");

            if (existingDetail != null)
            {
                var diff = detail.Quantity - existingDetail.Quantity;

                if (diff > 0)
                {
                    await stock.ReserveStockAsync(detail.ProductId, detail.WarehouseId, diff);
                }
                else if (diff < 0)
                {
                    await stock.ReturnStockAsync(detail.ProductId, detail.WarehouseId, -diff);
                }

                existingDetail.Quantity = detail.Quantity;
                existingDetail.Discount = detail.Discount;
                existingDetail.UnitMeasureId = unitMeasure.Id;
                existingDetail.UnitMeasure = unitMeasure;

                var netWeight = existingDetail.Quantity;
                var grossWeight = netWeight;

                var subtotal = existingDetail.Quantity * product.Price;
                var taxableBase = subtotal - existingDetail.Discount;
                var taxRate = product.Tax?.Rate ?? 0;
                var taxValue = taxableBase * (taxRate / 100);
                var total = taxableBase + taxValue;

                existingDetail.NetWeight = netWeight;
                existingDetail.GrossWeight = grossWeight;
                existingDetail.UnitPrice = product.Price;
                existingDetail.Subtotal = taxableBase;
                existingDetail.TaxValue = taxValue;
                existingDetail.Total = total;
            }
            else
            {
                var warehouse = await context.Warehouses
                .FirstOrDefaultAsync(w => w.Id == detail.WarehouseId);

                var stockRecord = await context.ProductWarehouses
                .FirstOrDefaultAsync(
                    pw =>
                    pw.ProductId == detail.ProductId &&
                    pw.WarehouseId == detail.WarehouseId);


                if (stockRecord == null || stockRecord.Stock < detail.Quantity)
                {
                    throw new InvalidOperationException($"Stock insuficiente para {product.Name} en bodega {warehouse!.Code}");
                }

                await stock.ReserveStockAsync(detail.ProductId, detail.WarehouseId, detail.Quantity);

                var netWeight = detail.Quantity;
                var grossWeight = netWeight;

                var subtotal = detail.Quantity * product.Price;
                var taxableBase = subtotal - detail.Discount;
                var taxRate = product.Tax?.Rate ?? 0;
                var taxValue = taxableBase * (taxRate / 100);
                var total = taxableBase + taxValue;

                var newDetail = new InvoiceDetail
                {
                    InvoiceId = invoice.Id,
                    ProductId = detail.ProductId,
                    WarehouseId = detail.WarehouseId,
                    UnitMeasureId = unitMeasure.Id,
                    UnitMeasure = unitMeasure,
                    Quantity = detail.Quantity,
                    NetWeight = netWeight,
                    GrossWeight = grossWeight,
                    UnitPrice = product.Price,
                    Discount = detail.Discount,
                    Subtotal = taxableBase,
                    TaxId = product.TaxId,
                    TaxRate = taxRate,
                    TaxValue = taxValue,
                    Total = total,
                    Product = product,
                    Warehouse = warehouse,
                    Tax = product.Tax
                };

                invoice.InvoiceDetails.Add(newDetail);
            }
        }
    }
}
