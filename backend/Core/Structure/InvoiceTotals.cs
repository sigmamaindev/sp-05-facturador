namespace Core.Structure;

public readonly record struct InvoiceTotals(
    decimal SubtotalWithoutTaxes,
    decimal SubtotalWithTaxes,
    decimal DiscountTotal,
    decimal TaxTotal
);
