using Core.DTOs;
using Core.DTOs.AtsDto;

namespace Core.Interfaces.Repository;

public interface IAtsRepository
{
    Task<ApiResponse<List<AtsPurchaseResDto>>> GetAtsPurchasesAsync(int year, int month);
    Task<ApiResponse<List<AtsSaleResDto>>> GetAtsSalesAsync(int year, int month);
    Task<ApiResponse<string>> GetAtsXmlAsync(int year, int month);
}
