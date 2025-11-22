using Core.DTOs;
using Core.DTOs.Certificate;

namespace Core.Interfaces.Repository;

public interface ICertificateRepository
{
    Task<ApiResponse<CertificateResDto>> SaveCertificateAsync(int businessId, byte[] pfxBytes, string password);
}
