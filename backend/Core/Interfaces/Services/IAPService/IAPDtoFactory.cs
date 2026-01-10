using Core.DTOs.APDto;
using Core.Entities;

namespace Core.Interfaces.Services.IAPService;

public interface IAPDtoFactory
{
    APSimpleResDto APSimpleRes(AccountsPayable accountsPayable);
    APComplexResDto APComplexRes(AccountsPayable accountsPayable);
}
