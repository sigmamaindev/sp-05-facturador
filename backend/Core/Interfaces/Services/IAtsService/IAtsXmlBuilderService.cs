using Core.DTOs.AtsDto;
using Core.Entities;

namespace Core.Interfaces.Services.IAtsService;

public interface IAtsXmlBuilderService
{
    string BuildAtsXml(int year, int month, Business business, string numEstabRuc,
        IEnumerable<AtsPurchaseResDto> purchases, IEnumerable<AtsSaleResDto> sales,
        decimal totalVentas);
}
