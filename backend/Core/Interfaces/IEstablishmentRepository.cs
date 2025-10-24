using Core.DTOs;
using Core.DTOs.Establishment;

namespace Core.Interfaces;

public interface IEstablishmentRepository
{
    Task<ApiResponse<List<EstablishmentResDto>>> GetEstablishmentsAsync(string? keyword, int page, int limit);
}
