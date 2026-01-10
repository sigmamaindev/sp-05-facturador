using Core.DTOs.APDto;
using Core.Entities;

namespace Core.Interfaces.Services.IAPService;

public interface IAccountsPayableService
{
    Task<AccountsPayable> UpsertFromPurchaseAsync(Purchase purchase, APCreateFromPurchaseReqDto apCreateFromPurchaseReqDto);
}
