using System;
using System.Collections.Generic;
using System.Linq;
using Core.DTOs;
using Core.DTOs.PurchaseDto;
using Core.Entities;
using Core.Interfaces.Kardex;
using Core.Interfaces.Purchases;
using Core.Interfaces.Specifications.PurchaseSpecification;
using Infrastructure.Data;
using Infrastructure.Specification.PurchaseSpecification;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Purchases;

public class PurchaseService(
    StoreContext context,
    IKardexService kardexService) : IPurchaseService
{
    public async Task<ApiResponse<PurchaseResDto>> CreatePurchaseAsync(PurchaseCreateReqDto purchaseCreateReqDto)
    {
        var response = new ApiResponse<PurchaseResDto>();

        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var purchase = new Purchase
            {
                BusinessId = purchaseCreateReqDto.BusinessId,
                EstablishmentId = purchaseCreateReqDto.EstablishmentId,
                WarehouseId = purchaseCreateReqDto.WarehouseId,
                SupplierId = purchaseCreateReqDto.SupplierId,
                PurchaseDate = purchaseCreateReqDto.PurchaseDate,
                DocumentNumber = purchaseCreateReqDto.DocumentNumber,
                Reference = purchaseCreateReqDto.Reference
            };

            var detailList = new List<PurchaseDetail>();

            foreach (var detail in purchaseCreateReqDto.Details)
            {
                var product = await context.Products
                    .Include(p => p.Tax)
                    .FirstOrDefaultAsync(p => p.Id == detail.ProductId)
                    ?? throw new Exception($"Producto {detail.ProductId} no encontrado");

                var warehouse = await context.Warehouses
                    .FirstOrDefaultAsync(w => w.Id == detail.WarehouseId)
                    ?? throw new Exception($"Bodega {detail.WarehouseId} no encontrada");

                var subtotal = detail.Quantity * detail.UnitCost;
                var taxRate = product.Tax?.Rate ?? detail.TaxRate;
                var taxValue = subtotal * (taxRate / 100);
                var total = subtotal + taxValue;

                var purchaseDetail = new PurchaseDetail
                {
                    ProductId = detail.ProductId,
                    WarehouseId = detail.WarehouseId,
                    Quantity = detail.Quantity,
                    UnitCost = detail.UnitCost,
                    Subtotal = subtotal,
                    TaxId = product.TaxId,
                    TaxRate = taxRate,
                    TaxValue = taxValue,
                    Total = total,
                    Product = product,
                    Warehouse = warehouse,
                    Tax = product.Tax
                };

                detailList.Add(purchaseDetail);
            }

            purchase.PurchaseDate = purchase.PurchaseDate == default ? DateTime.UtcNow : purchase.PurchaseDate;
            purchase.Subtotal = detailList.Sum(d => d.Subtotal);
            purchase.TotalTax = detailList.Sum(d => d.TaxValue);
            purchase.Total = purchase.Subtotal + purchase.TotalTax;
            purchase.Status = string.IsNullOrWhiteSpace(purchase.Status) ? "REGISTRADA" : purchase.Status;
            purchase.PurchaseDetails = detailList;

            context.Purchases.Add(purchase);
            await context.SaveChangesAsync();

            foreach (var detail in detailList)
            {
                await kardexService.IncreaseStockForPurchaseAsync(
                    detail.ProductId,
                    detail.WarehouseId,
                    detail.Quantity,
                    detail.UnitCost,
                    purchase.Id);
            }

            await transaction.CommitAsync();

            response.Success = true;
            response.Message = "Compra registrada correctamente";
            response.Data = MapToPurchaseResDto(purchase);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            response.Success = false;
            response.Message = "Error al registrar la compra";
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<ApiResponse<PurchaseResDto>> GetPurchaseByIdAsync(int purchaseId)
    {
        var response = new ApiResponse<PurchaseResDto>();

        try
        {
            IPurchaseByIdWithDetailsSpecification specification = new PurchaseByIdWithDetailsSpecification(purchaseId);
            var purchase = await specification.Apply(context.Purchases).FirstOrDefaultAsync();

            if (purchase == null)
            {
                response.Success = false;
                response.Message = "Compra no encontrada";
                response.Error = "No existe una compra con el ID especificado";
                return response;
            }

            response.Success = true;
            response.Message = "Compra obtenida correctamente";
            response.Data = MapToPurchaseResDto(purchase);
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al obtener la compra";
            response.Error = ex.Message;
        }

        return response;
    }

    private static PurchaseResDto MapToPurchaseResDto(Purchase purchase)
    {
        return new PurchaseResDto
        {
            Id = purchase.Id,
            BusinessId = purchase.BusinessId,
            EstablishmentId = purchase.EstablishmentId,
            WarehouseId = purchase.WarehouseId,
            SupplierId = purchase.SupplierId,
            PurchaseDate = purchase.PurchaseDate,
            DocumentNumber = purchase.DocumentNumber,
            Reference = purchase.Reference,
            Subtotal = purchase.Subtotal,
            TotalTax = purchase.TotalTax,
            Total = purchase.Total,
            Status = purchase.Status,
            Details = purchase.PurchaseDetails.Select(d => new PurchaseDetailResDto
            {
                ProductId = d.ProductId,
                WarehouseId = d.WarehouseId,
                TaxId = d.TaxId,
                Quantity = d.Quantity,
                UnitCost = d.UnitCost,
                Subtotal = d.Subtotal,
                TaxRate = d.TaxRate,
                TaxValue = d.TaxValue,
                Total = d.Total
            }).ToList()
        };
    }
}
