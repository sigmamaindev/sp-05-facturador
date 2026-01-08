using Core.DTOs.SupplierDto;

namespace Core.DTOs.PurchaseDto;

public class PurchaseSimpleResDto
{
    public int Id { get; set; }
    public string Environment { get; set; } = string.Empty;
    public string EmissionTypeCode { get; set; } = string.Empty;
    public string BusinessName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string ReceiptType { get; set; } = string.Empty;
    public string SupportingCode { get; set; } = string.Empty;
    public string SupportingDocumentCode { get; set; } = string.Empty;
    public string EstablishmentCode { get; set; } = string.Empty;
    public string EmissionPointCode { get; set; } = string.Empty;
    public string Sequential { get; set; } = string.Empty;
    public string MainAddress { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public string? EstablishmentAddress { get; set; }
    public string? SpecialTaxpayer { get; set; }
    public string? MandatoryAccounting { get; set; }
    public string TypeDocumentSubjectDetained { get; set; } = string.Empty;
    public string TypeSubjectDetained { get; set; } = string.Empty;
    public string RelatedParty { get; set; } = string.Empty;
    public string BusinessNameSubjectDetained { get; set; } = string.Empty;
    public string DocumentSubjectDetained { get; set; } = string.Empty;
    public string FiscalPeriod { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public SupplierResDto? Supplier { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsElectronic { get; set; }
    public string? AuthorizationNumber { get; set; }
    public DateTime? AuthorizationDate { get; set; }
    public decimal SubtotalWithoutTaxes { get; set; }
    public decimal SubtotalWithTaxes { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal TotalPurchase { get; set; }
}
