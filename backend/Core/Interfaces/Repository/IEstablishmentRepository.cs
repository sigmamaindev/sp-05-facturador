using Core.DTOs;
using Core.DTOs.EstablishmentDto;

namespace Core.Interfaces.Repository;

public interface IEstablishmentRepository
{
    Task<ApiResponse<List<EstablishmentResDto>>> GetEstablishmentsAsync(string? keyword, int page, int limit);
    Task<ApiResponse<EstablishmentResDto>> GetEstablishmentByIdAsync(int id);
    Task<ApiResponse<EstablishmentResDto>> CreateEstablishmentAsync(EstablishmentReqDto establishmentReqDto);
    Task<ApiResponse<EstablishmentResDto>> UpdateEstablishmentAsync(int establishmentId, EstablishmentReqDto establishmentReqDto);
}
