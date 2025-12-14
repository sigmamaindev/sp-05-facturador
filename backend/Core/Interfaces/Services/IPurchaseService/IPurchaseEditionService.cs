using Core.DTOs.PurchaseDto;
using Core.Entities;

namespace Core.Interfaces.Services.IPurchaseService;

public interface IPurchaseEditionService
{
    Purchase BuildPurchase(PurchaseCreateReqDto dto, Supplier supplier, Business business, Establishment establishment, EmissionPoint emissionPoint, User user, DateTime purchaseDate);
    Task AddPurchaseDetailsAsync(Purchase purchase, IEnumerable<PurchaseDetailCreateReqDto> details);
}
