using Core.DTOs.AtsDto;
using Core.Entities;

namespace Core.Interfaces.Services.IAtsService;

public interface IAtsXmlBuilderService
{
    string BuildAtsPurchasesXml(int year, int month, Business business, IEnumerable<AtsPurchaseResDto> purchases);
}

