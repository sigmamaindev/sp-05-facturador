using Core.Constants;

namespace Core.Entities;

public class Purchase : BaseEntity
{
    public string Environment { get; set; } = string.Empty;
    public string EmissionTypeCode { get; set; } = EmissionType.NORMAL;
    public required string BusinessName { get; set; }
    public string Name { get; set; } = string.Empty;
    public required string Document { get; set; }
    public string AccessKey { get; set; } = string.Empty;
    public string ReceiptType { get; set; } = ReceiptCodeType.WITHHOLDING_RECEIPT;
    public required string EstablishmentCode { get; set; }
    public required string EmissionPointCode { get; set; }
    public required string Sequential { get; set; }
    public required string MainAddress { get; set; }
    public DateTime IssueDate { get; set; }                            
    public string? EstablishmentAddress { get; set; }                  
    public string? SpecialTaxpayer { get; set; }
    public string? MandatoryAccounting { get; set; }
    public string TypeDocumentSubjectDetained { get; set; } = DocumentType.RUC; 
    public required string TypeSubjectDetained { get; set; }
    public string RelatedParty { get; set; } = "NO";
    public required string BusinessNameSubjectDetained { get; set; }
    public required string DocumentSubjectDetained { get; set; }
    public required string FiscalPeriod { get; set; }
    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsElectronic { get; set; }
    public string? AuthorizationNumber { get; set; }
    public DateTime? AuthorizationDate { get; set; }
    public decimal SubtotalWithoutTaxes { get; set; }
    public decimal SubtotalWithTaxes { get; set; }
    public decimal DiscountTotal { get; set; }
    public decimal TaxTotal { get; set; }
    public decimal TotalPurchase { get; set; }
    public ICollection<PurchaseDetail> PurchaseDetails { get; set; } = [];
}
