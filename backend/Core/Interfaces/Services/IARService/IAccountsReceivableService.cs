using Core.DTOs.ARDto;
using Core.Entities;

namespace Core.Interfaces.Services.IARService;

public interface IAccountsReceivableService
{
    Task<AccountsReceivable> UpsertFromInvoiceAsync(Invoice invoice, ARCreateFromInvoiceReqDto aRCreateFromInvoiceReqDto);
}
