using Core.Entities;
using Core.DTOs.InvoiceDto;
using Core.DTOs.CustomerDto;
using Core.Interfaces.Services.IInvoiceService;

namespace Infrastructure.Services.InvoiceService;

public class InvoiceDtoFactory : IInvoiceDtoFactory
{
    public InvoiceComplexResDto InvoiceComplexRes(Invoice invoice)
    {
        return new InvoiceComplexResDto
        {
            Id = invoice.Id,
            Sequential = invoice.Sequential,
            AccessKey = invoice.AccessKey,
            AuthorizationNumber = invoice.AuthorizationNumber,
            Environment = invoice.Environment,
            ReceiptType = invoice.ReceiptType,
            Status = invoice.Status,
            IsElectronic = invoice.IsElectronic,
            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            BusinessId = invoice.Business!.Id,
            BusinessDocument = invoice.Business.Document,
            BusinessName = invoice.Business.Name,
            EstablishmentId = invoice.Establishment!.Id,
            EstablishmentCode = invoice.Establishment.Code,
            EstablishmentName = invoice.Establishment.Name,
            EmissionPointId = invoice.EmissionPoint!.Id,
            EmissionPointCode = invoice.EmissionPoint.Code,
            EmissionPointDescription = invoice.EmissionPoint.Description,
            UserId = invoice.User!.Id,
            UserDocument = invoice.User.Document,
            UserFullName = invoice.User.FullName,
            Customer = new CustomerResDto
            {
                Id = invoice.Customer!.Id,
                DocumentType = invoice.Customer.DocumentType,
                Document = invoice.Customer.Document,
                Email = invoice.Customer.Email,
                Name = invoice.Customer.Name,
                Cellphone = invoice.Customer.Cellphone,
                Telephone = invoice.Customer.Telephone,
                Address = invoice.Customer.Address,
                IsActive = invoice.Customer.IsActive,
                CreatedAt = invoice.Customer.CreatedAt
            },
            SubtotalWithoutTaxes = invoice.SubtotalWithoutTaxes,
            SubtotalWithTaxes = invoice.SubtotalWithTaxes,
            DiscountTotal = invoice.DiscountTotal,
            TaxTotal = invoice.TaxTotal,
            TotalInvoice = invoice.TotalInvoice,
            PaymentMethod = invoice.PaymentMethod,
            PaymentTermDays = invoice.PaymentTermDays,
            Description = invoice.Description ?? string.Empty,
            AdditionalInformation = invoice.AdditionalInformation,
            AuthorizationDate = invoice.AuthorizationDate,
            SriMessage = invoice.SriMessage,
            XmlSigned = invoice.XmlSigned ?? string.Empty,
            Details = [.. invoice.InvoiceDetails.Select(d => new InvoiceDetailResDto
            {
                Id = d.Id,
                InvoiceId = d.InvoiceId,
                ProductId = d.Product!.Id,
                ProductCode = d.Product.Sku,
                ProductName = d.Product.Name,
                UnitMeasureId = d.Product.UnitMeasureId,
                UnitMeasureCode = d.Product.UnitMeasure!.Code,
                UnitMeasureName = d.Product.UnitMeasure!.Name,
                WarehouseId = d.Warehouse!.Id,
                WarehouseCode = d.Warehouse.Code,
                WarehouseName = d.Warehouse.Name,
                TaxId = d.Tax!.Id,
                TaxCode = d.Tax.Code,
                TaxName = d.Tax.Name,
                TaxRate = d.TaxRate,
                TaxValue = d.TaxValue,
                Quantity = d.Quantity,
                UnitPrice = d.UnitPrice,
                Discount = d.Discount,
                Subtotal = d.Subtotal,
                Total = d.Total
            })]
        };
    }

    public InvoiceSimpleResDto InvoiceSimpleRes(Invoice invoice)
    {
        return new InvoiceSimpleResDto
        {
            Id = invoice.Id,
            Sequential = invoice.Sequential,
            AccessKey = invoice.AccessKey,
            AuthorizationNumber = invoice.AuthorizationNumber,
            Environment = invoice.Environment,
            ReceiptType = invoice.ReceiptType,
            Status = invoice.Status,
            IsElectronic = invoice.IsElectronic,
            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            BusinessId = invoice.Business!.Id,
            BusinessDocument = invoice.Business.Document,
            BusinessName = invoice.Business.Name,
            EstablishmentId = invoice.Establishment!.Id,
            EstablishmentCode = invoice.Establishment.Code,
            EstablishmentName = invoice.Establishment.Name,
            EmissionPointId = invoice.EmissionPoint!.Id,
            EmissionPointCode = invoice.EmissionPoint.Code,
            EmissionPointDescription = invoice.EmissionPoint.Description,
            UserId = invoice.User!.Id,
            UserDocument = invoice.User.Document,
            UserFullName = invoice.User.FullName,
            Customer = new CustomerResDto
            {
                Id = invoice.Customer!.Id,
                DocumentType = invoice.Customer.DocumentType,
                Document = invoice.Customer.Document,
                Email = invoice.Customer.Email,
                Name = invoice.Customer.Name,
                Cellphone = invoice.Customer.Cellphone,
                Telephone = invoice.Customer.Telephone,
                Address = invoice.Customer.Address,
                IsActive = invoice.Customer.IsActive,
                CreatedAt = invoice.Customer.CreatedAt
            },
            SubtotalWithoutTaxes = invoice.SubtotalWithoutTaxes,
            SubtotalWithTaxes = invoice.SubtotalWithTaxes,
            DiscountTotal = invoice.DiscountTotal,
            TaxTotal = invoice.TaxTotal,
            TotalInvoice = invoice.TotalInvoice,
            PaymentMethod = invoice.PaymentMethod,
            PaymentTermDays = invoice.PaymentTermDays,
            Description = invoice.Description ?? string.Empty,
            AdditionalInformation = invoice.AdditionalInformation,
            AuthorizationDate = invoice.AuthorizationDate,
            SriMessage = invoice.SriMessage,
            XmlSigned = invoice.XmlSigned ?? string.Empty
        };
    }
}
