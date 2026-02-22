using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Core.Interfaces.Repository;
using Core.DTOs;
using Core.DTOs.CertificateDto;
using Core.Interfaces.Services.IUtilService;

namespace Infrastructure.Data;

public class CertificateRepository(StoreContext context, IAesEncryptionService aes) : ICertificateRepository
{
    public async Task<ApiResponse<CertificateResDto>> SaveCertificateAsync(int businessId, byte[] pfxBytes, string password, string? fileName)
    {
        var response = new ApiResponse<CertificateResDto>();

        try
        {
            var base64 = Convert.ToBase64String(pfxBytes);
            var encryptedPwd = aes.Encrypt(password);

            var existing = await context.BusinessCertificates
            .FirstOrDefaultAsync(c => c.BusinessId == businessId);

            if (existing == null)
            {
                existing = new BusinessCertificate
                {
                    BusinessId = businessId,
                    CertificateBase64 = base64,
                    Password = encryptedPwd,
                    FileName = fileName,
                    CreatedAt = DateTime.UtcNow
                };

                context.BusinessCertificates.Add(existing);
            }
            else
            {
                existing.CertificateBase64 = base64;
                existing.Password = encryptedPwd;
                existing.FileName = fileName;
                existing.CreatedAt = DateTime.UtcNow;
            }

            await context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Certificado guardado correctamente.";
            response.Data = new CertificateResDto
            {
                Id = existing.Id,
                BusinessId = existing.BusinessId,
                FileName = existing.FileName,
                CreatedAt = existing.CreatedAt
            };

            return response;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al guardar el certificado.";
            response.Error = ex.Message;

            return response;
        }
    }

    public async Task<ApiResponse<CertificateResDto>> GetCertificateByBusinessIdAsync(int businessId)
    {
        var response = new ApiResponse<CertificateResDto>();

        try
        {
            var existing = await context.BusinessCertificates
                .FirstOrDefaultAsync(c => c.BusinessId == businessId);

            if (existing == null)
            {
                response.Success = true;
                response.Message = "No se encontró un certificado para esta empresa.";
                response.Data = null;
                return response;
            }

            response.Success = true;
            response.Data = new CertificateResDto
            {
                Id = existing.Id,
                BusinessId = existing.BusinessId,
                FileName = existing.FileName,
                CreatedAt = existing.CreatedAt
            };

            return response;
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Message = "Error al consultar el certificado.";
            response.Error = ex.Message;

            return response;
        }
    }
}
