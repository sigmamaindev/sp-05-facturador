using Core.DTOs;
using Core.DTOs.EmissionPoint;

namespace Core.Interfaces;

public interface IEmissionPointRepository
{
    Task<ApiResponse<List<EmissionPointResDto>>> GetEmissionPointsAsync(string? keyword, int page, int limit);
}
