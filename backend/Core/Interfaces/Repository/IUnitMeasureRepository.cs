using Core.DTOs;
using Core.DTOs.UnitMeasureDto;

namespace Core.Interfaces.Repository;

public interface IUnitMeasureRepository
{
    Task<ApiResponse<List<UnitMeasureResDto>>> GetUnitMeasuresAsync(string? keyword, int page, int limit);
    Task<ApiResponse<UnitMeasureResDto>> GetUnitMeasureByIdAsync(int id);
}
