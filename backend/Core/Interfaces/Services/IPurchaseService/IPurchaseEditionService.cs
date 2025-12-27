using Core.DTOs.PurchaseDto;
using Core.Entities;

namespace Core.Interfaces.Services.IPurchaseService;

public interface IPurchaseEditionService
{
    Purchase BuildPurchase(PurchaseCreateReqDto dto, Business business, Supplier supplier, DateTime purchaseDate);
    Task AddPurchaseDetailsAsync(Purchase purchase, IEnumerable<PurchaseDetailCreateReqDto> details);
}
