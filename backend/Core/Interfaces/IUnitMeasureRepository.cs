using Core.DTOs;
using Core.DTOs.UnitMeasure;

namespace Core.Interfaces;

public interface IUnitMeasureRepository
{
    Task<ApiResponse<List<UnitMeasureResDto>>> GetUnitMeasuresAsync(string? keyword, int page, int limit);
}
