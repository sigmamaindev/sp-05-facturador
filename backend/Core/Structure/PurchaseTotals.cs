namespace Core.Structure;

public readonly record struct PurchaseTotals(
    decimal SubtotalWithoutTaxes,
    decimal SubtotalWithTaxes,
    decimal DiscountTotal,
    decimal TaxTotal
);
