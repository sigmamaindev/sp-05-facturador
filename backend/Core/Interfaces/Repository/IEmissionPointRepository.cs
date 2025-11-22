using Core.DTOs;
using Core.DTOs.EmissionPoint;

namespace Core.Interfaces.Repository;

public interface IEmissionPointRepository
{
    Task<ApiResponse<List<EmissionPointResDto>>> GetEmissionPointsAsync(int establishmentId, string? keyword, int page, int limit);
    Task<ApiResponse<EmissionPointResDto>> GetEmissionPointByIdAsync(int establishmentId, int id);
    Task<ApiResponse<EmissionPointResDto>> CreateEmissionPointAsync(int establishmentId, EmissionPointCreateReqDto emissionPointCreateReqDto);
    Task<ApiResponse<EmissionPointResDto>> UpdateEmissionPointAsync(int emissionPointId, EmissionPointUpdateReqDto emissionPointUpdateReqDto);
}
