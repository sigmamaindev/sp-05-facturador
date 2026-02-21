using Core.DTOs.SriDto;

namespace Core.Interfaces.Services.ISriService;

public interface ISriPersonLookupService
{
    Task<SriPersonaResDto?> LookupByDocumentAsync(string document);
}
