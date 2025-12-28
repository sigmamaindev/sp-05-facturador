using Core.Entities;
using Core.DTOs.ARDto;

namespace Core.Interfaces.Services.IARService;

public interface IARDtoFactory
{
    ARSimpleResDto ARSimpleRes(AccountsReceivable accountsReceivable);
    ARComplexResDto ARComplexRes(AccountsReceivable accountsReceivable);
}
