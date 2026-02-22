using Core.DTOs;
using Core.DTOs.CertificateDto;

namespace Core.Interfaces.Repository;

public interface ICertificateRepository
{
    Task<ApiResponse<CertificateResDto>> SaveCertificateAsync(int businessId, byte[] pfxBytes, string password, string? fileName);
    Task<ApiResponse<CertificateResDto>> GetCertificateByBusinessIdAsync(int businessId);
}
