using Core.Entities;
using Core.Structure;

namespace Core.Interfaces.Services.IPurchaseService;

public interface IPurchaseCalculationService
{
    PurchaseTotals Calculate(Purchase purchase);
}
